using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CC.Core;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using CC.Core.ValidationServices;
using CC.Utility;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Rules;
using MF.Core.Services;
using MF.Web.Areas.Schedule.Models.BulkAction;
using MF.Web.Config;
using MF.Web.Controllers;
using MF.Web;
using StructureMap;
using CC.Core.CustomAttributes;

namespace MF.Web.Areas.Billing.Controllers
{
    public class PaymentController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ISessionContext _sessionContext;
        private readonly IClientPaymentToSessions _clientPaymentToSessions;

        public PaymentController(IRepository repository,
            ISaveEntityService saveEntityService,
            ISessionContext sessionContext,
            IClientPaymentToSessions clientPaymentToSessions)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
            _clientPaymentToSessions = clientPaymentToSessions;
        }

        public ActionResult AddUpdate_Template(ViewModel input)
        {
            return View("AddUpdate", new PaymentViewModel());
        }

        public CustomJsonResult AddUpdate(ViewModel input)
        {
//            var payment = input.EntityId > 0 ? _repository.Find<Payment>(input.EntityId) : new Payment();
            var client = _repository.Find<Client>(input.ParentId);
            var baseSessionRate = _repository.Query<BaseSessionRate>().FirstOrDefault();
            var payment = input.EntityId > 0 ? client.Payments.FirstOrDefault(x => x.EntityId == input.EntityId) : new Payment();
            var sessionRatesDto = new SessionRatesDto
                                      {
                                        FullHour = client.SessionRates.FullHour > 0 ? client.SessionRates.FullHour : baseSessionRate.FullHour,
                                        HalfHour = client.SessionRates.HalfHour > 0 ? client.SessionRates.HalfHour : baseSessionRate.HalfHour,
                                        FullHourTenPack = client.SessionRates.FullHourTenPack > 0 ? client.SessionRates.FullHourTenPack : baseSessionRate.FullHourTenPack,
                                        HalfHourTenPack = client.SessionRates.HalfHourTenPack > 0 ? client.SessionRates.HalfHourTenPack : baseSessionRate.HalfHourTenPack,
                                        Pair = client.SessionRates.Pair > 0 ? client.SessionRates.Pair : baseSessionRate.Pair,
                                        PairTenPack = client.SessionRates.PairTenPack > 0 ? client.SessionRates.PairTenPack : baseSessionRate.PairTenPack
                                      };
            //hijacking sessionratesdto since I need exact same object just different name
            var clientSessionsDto = new SessionRatesDto
                                        {
                                            FullHour = client.Sessions.Any(x => x.AppointmentType == AppointmentType.Hour.ToString() && x.InArrears)
                                                ? -client.Sessions.Count(x => x.AppointmentType == AppointmentType.Hour.ToString() && x.InArrears)
                                                : client.Sessions.Count(x => x.AppointmentType == AppointmentType.Hour.ToString() && !x.SessionUsed),
                                            HalfHour = client.Sessions.Any(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && x.InArrears)
                                                ? -client.Sessions.Count(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && x.InArrears)
                                                : client.Sessions.Count(x => x.AppointmentType == AppointmentType.HalfHour.ToString() && !x.SessionUsed),
                                            Pair = client.Sessions.Any(x => x.AppointmentType == AppointmentType.Pair.ToString() && x.InArrears)
                                                ? -client.Sessions.Count(x => x.AppointmentType == AppointmentType.Pair.ToString() && x.InArrears)
                                                : client.Sessions.Count(x => x.AppointmentType == AppointmentType.Pair.ToString() && !x.SessionUsed),
                                        };
            var model = Mapper.Map<Payment,PaymentViewModel>(payment);
            model._sessionRateDto = sessionRatesDto;
            model._sessionsAvailable = clientSessionsDto;
            model._Title = WebLocalizationKeys.PAYMENT_INFORMATION.ToFormat(client.FullNameFNF);
            model._deleteUrl = UrlContext.GetUrlForAction<PaymentController>(x=>x.Delete(null));
            model.ParentId = client.EntityId;
            model._saveUrl = UrlContext.GetUrlForAction<PaymentController>(x => x.Save(null));
            return new CustomJsonResult(model);
        }

        public ActionResult Display_Template(ViewModel input)
        {
            return View("Display", new PaymentViewModel());
        }

        public CustomJsonResult Display(ViewModel input)
        {
            var client = _repository.Find<Client>(input.ParentId);
            var payment =  client.Payments.FirstOrDefault(x=>x.EntityId == input.EntityId);
            var model = Mapper.Map<Payment, PaymentViewModel>(payment);
            model._Title = WebLocalizationKeys.PAYMENT_INFORMATION.ToFormat(client.FullNameFNF);
            return new CustomJsonResult(model);
        }

        public CustomJsonResult Delete(ViewModel input)
        {
            var client = _repository.Find<Client>(input.ParentId);
            var payment = client.Payments.FirstOrDefault(x => x.EntityId == input.EntityId);
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeletePaymentRules");
            var rulesResult = rulesEngineBase.ExecuteRules(payment);
            if (rulesResult.GetLastValidationReport().Success)
            {
                removeSessionsByPayment(client, payment);
                client.RemovePayment(payment);
                payment.Client = null;
                var validationManager = _saveEntityService.ProcessSave(client);
                var saveClientNotification = validationManager.Finish();
                return new CustomJsonResult(saveClientNotification);
            }
            var notification = rulesResult.Finish();
            return new CustomJsonResult(notification);
        }

        private void removeSessionsByPayment(Client client, Payment payment)
        {
            var sessions = client.Sessions.Where(x => x.PurchaseBatchNumber == payment.PaymentBatchId.ToString());
            sessions.Where(x => x.SessionUsed).ForEachItem(y =>
            {
                y.InArrears = true;
                y.PurchaseBatchNumber = string.Empty;
            });
            sessions.Where(x => !x.SessionUsed).ForEachItem(client.RemoveSession);
        }

        public CustomJsonResult DeleteMultiple(BulkActionViewModel input)
        {
            var client = _repository.Find<Client>(input.ParentId);
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeletePaymentRules");
            IValidationManager validationManager = new ValidationManager(_repository);
            
            input.EntityIds.ForEachItem(x =>
            {
                var payment = client.Payments.FirstOrDefault(i => i.EntityId == x);
                validationManager = rulesEngineBase.ExecuteRules(payment, validationManager);
                var report = validationManager.GetLastValidationReport();
                if (report.Success)
                {
                    removeSessionsByPayment(client, payment);
                    client.RemovePayment(payment);
                    payment.Client = null;
                }
            });
            if (validationManager.GetLastValidationReport().Success)
            {
                var processSave = _saveEntityService.ProcessSave(client);
                var saveClientNotification = processSave.Finish();
                return new CustomJsonResult(saveClientNotification);
            }
            var notification = validationManager.Finish();
            return new CustomJsonResult(notification);
        }

        public CustomJsonResult Save(PaymentViewModel input)
        {
            var client = _repository.Find<Client>(input.ParentId);
            Payment payment;
            if (input.EntityId > 0)
            {
                payment = client.Payments.FirstOrDefault(x=>x.EntityId== input.EntityId);
            }
            else
            {
                payment = new Payment{Client = client, PaymentBatchId = Guid.NewGuid()};
            }
            payment = mapToDomain(input, payment);
            client = _clientPaymentToSessions.Execute(client, payment);
            client.AddPayment(payment);
            var crudManager = _saveEntityService.ProcessSave(client);

            var notification = crudManager.Finish();
            return new CustomJsonResult(notification){ ContentType = "text/plain" };

        }


        private Payment mapToDomain(PaymentViewModel model, Payment payment)
        {
            payment.FullHourTenPack = model.FullHourTenPack;
            payment.FullHourTenPackPrice = model.FullHourTenPack > 0 ? model.FullHourTenPackPrice / model.FullHourTenPack : 0;
            payment.FullHour = model.FullHour;
            payment.FullHourPrice = model.FullHour > 0 ? model.FullHourPrice / model.FullHour : 0;
            payment.HalfHourTenPack = model.HalfHourTenPack;
            payment.HalfHourTenPackPrice = model.HalfHourTenPack > 0 ? model.HalfHourTenPackPrice / model.HalfHourTenPack : 0;
            payment.HalfHour = model.HalfHour;
            payment.HalfHourPrice = model.HalfHour > 0 ? model.HalfHourPrice / model.HalfHour : 0;
            payment.Pair = model.Pair;
            payment.PairPrice = model.Pair > 0 ? model.PairPrice / model.Pair : 0;
            payment.PairTenPack = model.PairTenPack;
            payment.PairTenPackPrice = model.PairTenPack > 0 ? model.PairTenPackPrice / model.PairTenPack : 0;
            payment.PaymentTotal = model.PaymentTotal;
            payment.Notes = model.Notes;
            return payment;
        }
    }

    public class SessionRatesDto
    {
        public double FullHour { get; set; }
        public double HalfHour { get; set; }
        public double FullHourTenPack { get; set; }
        public double HalfHourTenPack { get; set; }
        public double Pair { get; set; }
        public double PairTenPack { get; set; }
    }

    public class PaymentViewModel:ViewModel
    {
        public string _deleteUrl { get; set; }
        public double Total { get; set; }
        public SessionRatesDto _sessionRateDto { get; set; }
        public SessionRatesDto _sessionsAvailable { get; set; }

        public int FullHour { get; set; }
        public int HalfHour { get; set; }
        public int FullHourTenPack { get; set; }
        public int HalfHourTenPack { get; set; }
        public int Pair { get; set; }
        public int PairTenPack { get; set; }
        public double PaymentTotal { get; set; }
        public double FullHourTenPackPrice { get; set; }
        public double FullHourPrice { get; set; }
        public double HalfHourTenPackPrice { get; set; }
        public double HalfHourPrice { get; set; }
        public double PairPrice { get; set; }
        public double PairTenPackPrice { get; set; }
        [TextArea]
        public string Notes { get; set; }
    }
}   