using System;
using System.Linq;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
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

        public ActionResult AddUpdate(ViewModel input)
        {
            var client = _repository.Find<Client>(input.ParentId);
            var payment = input.EntityId > 0 ? client.Payments.FirstOrDefault(x => x.EntityId == input.EntityId) : new Payment { Client = client };
            // have to map this for some stupid reason I'm getting circular reference.
            var sessionRatesDto = new SessionRatesDto
                                      {
                                          FullHour = client.SessionRates.FullHour,
                                          HalfHour = client.SessionRates.HalfHour,
                                          FullHourTenPack = client.SessionRates.FullHourTenPack,
                                          HalfHourTenPack = client.SessionRates.HalfHourTenPack,
                                          Pair = client.SessionRates.Pair,
                                      };
            var model = new PaymentViewModel
            {
                Item = payment,
                SessionRateDto = sessionRatesDto,
                Title = WebLocalizationKeys.CLIENT_INFORMATION.ToString(),
                DeleteUrl = UrlContext.GetUrlForAction<PaymentController>(x=>x.Delete(null)),
                ParentId = client.EntityId
            };
            return View(model);
        }

        public ActionResult Delete(ViewModel input)
        {
            var payment = _repository.Find<Payment>(input.EntityId);
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeletePaymentRules");
            var rulesResult = rulesEngineBase.ExecuteRules(payment);
            if (!rulesResult.Success)
            {
                Notification notification = new Notification(rulesResult);
                return Json(notification,JsonRequestBehavior.AllowGet);
            }
            _repository.Delete(payment);
            _repository.UnitOfWork.Commit();
            return Json(new Notification{Success = true}, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeleteMultiple(BulkActionViewModel input)
        {
            input.EntityIds.Each(x =>
            {
                var item = _repository.Find<Payment>(x);
                _repository.Delete(item);
            });
            _repository.Commit();
            return Json(new Notification { Success = true }, JsonRequestBehavior.AllowGet);
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
            var paymentModel = model.Item;
            payment.FullHourTenPacks = paymentModel.FullHourTenPacks;
            payment.FullHourTenPacksPrice = paymentModel.FullHourTenPacksPrice;
            payment.FullHours = paymentModel.FullHours;
            payment.FullHoursPrice = paymentModel.FullHoursPrice;
            payment.HalfHourTenPacks = paymentModel.HalfHourTenPacks;
            payment.HalfHourTenPacksPrice = paymentModel.HalfHourTenPacksPrice;
            payment.HalfHours = paymentModel.HalfHours;
            payment.HalfHoursPrice = paymentModel.HalfHoursPrice;
            payment.Pairs = paymentModel.Pairs;
            payment.PairsPrice = paymentModel.PairsPrice;
            payment.PaymentTotal= paymentModel.PaymentTotal;
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
    }

    public class PaymentViewModel:ViewModel
    {
        public Payment Item { get; set; }
        public string DeleteUrl { get; set; }
        public double Total { get; set; }

        public SessionRatesDto SessionRateDto { get; set; }
    }
}