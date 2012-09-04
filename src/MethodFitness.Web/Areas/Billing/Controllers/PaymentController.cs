using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Rules;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Portfolio.Models.BulkAction;
using MethodFitness.Web.Controllers;
using NHibernate.Linq;
using StructureMap;

namespace MethodFitness.Web.Areas.Billing.Controllers
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

        public ActionResult AddUpdate(ViewModel input)
        {
//            var payment = input.EntityId > 0 ? _repository.Find<Payment>(input.EntityId) : new Payment();
            var client = _repository.Find<Client>(input.ParentId);
            var payment = input.EntityId>0 ?client.Payments.FirstOrDefault(x => x.EntityId == input.EntityId):new Payment();
            var sessionRatesDto = new SessionRatesDto
                                      {
                                          FullHour = client.SessionRates.FullHour > 0 ? client.SessionRates.FullHour : client.SessionRates.ResetFullHourRate(),
                                          HalfHour = client.SessionRates.HalfHour > 0 ? client.SessionRates.HalfHour : client.SessionRates.ResetHalfHourRate(),
                                          FullHourTenPack = client.SessionRates.FullHourTenPack > 0 ? client.SessionRates.FullHourTenPack : client.SessionRates.ResetFullHourTenPackRate(),
                                          HalfHourTenPack = client.SessionRates.HalfHourTenPack > 0 ? client.SessionRates.HalfHourTenPack : client.SessionRates.ResetHalfHourTenPackRate(),
                                          Pair = client.SessionRates.Pair > 0 ? client.SessionRates.Pair : client.SessionRates.ResetPairRate(),
                                          PairTenPack = client.SessionRates.PairTenPack > 0 ? client.SessionRates.PairTenPack : client.SessionRates.ResetPairTenPackRate(),
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
            model._Title = WebLocalizationKeys.PAYMENT_INFORMATION.ToString();
            model._deleteUrl = UrlContext.GetUrlForAction<PaymentController>(x=>x.Delete(null));
            model.ParentId = client.EntityId;
            model._saveUrl = UrlContext.GetUrlForAction<PaymentController>(x => x.Save(null));
            return Json(model,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Display_Template(ViewModel input)
        {
            return View("Display", new PaymentViewModel());
        }

        public ActionResult Display(ViewModel input)
        {
//            var payment =  _repository.Find<Payment>(input.EntityId);
//            var model = Mapper.Map<Payment, PaymentViewModel>(payment);
//            model._Title = WebLocalizationKeys.PAYMENT_INFORMATION.ToString();
//            return Json(model,JsonRequestBehavior.AllowGet);
            return null;
        }

        public ActionResult Delete(ViewModel input)
        {
//            var payment = _repository.Find<Payment>(input.EntityId);
//            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeletePaymentRules");
//            var rulesResult = rulesEngineBase.ExecuteRules(payment);
//            if (rulesResult.GetLastValidationReport().Success)
//            {
//                _repository.Delete(payment);
//            }
//            var notification = rulesResult.FinishWithAction();
//            return Json(notification, JsonRequestBehavior.AllowGet);
            return null;

        }

        public ActionResult DeleteMultiple(BulkActionViewModel input)
        {
//            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeletePaymentRules");
//            IValidationManager<Payment> validationManager = new ValidationManager<Payment>(_repository);
//            input.EntityIds.ForEachItem(x =>
//            {
//                var payment = _repository.Find<Payment>(input.EntityId);
//                validationManager = rulesEngineBase.ExecuteRules(payment, validationManager);
//                var report = validationManager.GetLastValidationReport();
//                if (report.Success)
//                {
//                    report.SuccessAction = a => _repository.Delete(a);
//                }
//            });
//            var notification = validationManager.FinishWithAction();
//            return Json(notification, JsonRequestBehavior.AllowGet);
            return null;
        }

        public ActionResult Save(PaymentViewModel input)
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
            return Json(notification, "text/plain");
        }


        private Payment mapToDomain(PaymentViewModel model, Payment payment)
        {
            payment.FullHourTenPacks = model.FullHourTenPacks;
            payment.FullHourTenPacksPrice = model.FullHourTenPacksPrice;
            payment.FullHours = model.FullHours;
            payment.FullHoursPrice = model.FullHoursPrice;
            payment.HalfHourTenPacks = model.HalfHourTenPacks;
            payment.HalfHourTenPacksPrice = model.HalfHourTenPacksPrice;
            payment.HalfHours = model.HalfHours;
            payment.HalfHoursPrice = model.HalfHoursPrice;
            payment.Pairs = model.Pairs;
            payment.PairsPrice = model.PairsPrice;
            payment.PairsTenPack = model.PairsTenPack;
            payment.PairsTenPackPrice = model.PairsTenPackPrice;
            payment.PaymentTotal = model.PaymentTotal;
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

        public int FullHours { get; set; }
        public int HalfHours { get; set; }
        public int FullHourTenPacks { get; set; }
        public int HalfHourTenPacks { get; set; }
        public int Pairs { get; set; }
        public int PairsTenPack { get; set; }
        public double PaymentTotal { get; set; }

        public double FullHourTenPacksPrice { get; set; }

        public double FullHoursPrice { get; set; }

        public double HalfHourTenPacksPrice { get; set; }

        public double HalfHoursPrice { get; set; }

        public double PairsPrice { get; set; }

        public double PairsTenPackPrice { get; set; }
    }
}