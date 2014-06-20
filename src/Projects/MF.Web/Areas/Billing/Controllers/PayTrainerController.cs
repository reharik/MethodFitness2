using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using CC.Core.ValidationServices;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Web.Config;
using MF.Web.Controllers;
using MF.Web;
using NHibernate.Linq;

namespace MF.Web.Areas.Billing.Controllers
{
    public class PayTrainerController:MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;

        public PayTrainerController(IRepository repository,
            ISaveEntityService saveEntityService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
        }

        public CustomJsonResult PayTrainer(PayTrainerViewModel input)
        {
            Notification notification;
            var trainer = _repository.Find<User>(input.EntityId);
            var trainerPayment = trainer.PayTrainer(input.eligableRows, input.paymentAmount);
            if(trainerPayment==null)
            {
                notification = new Notification {Success = false, Message = WebLocalizationKeys.YOU_MUST_SELECT_AT_LEAST_ONE_SESSION.ToString()};
                return new CustomJsonResult(notification);
            }
            var crudManager = _saveEntityService.ProcessSave(trainer);
            var continuation = crudManager.Finish();
            notification = new Notification(continuation);
            notification.Variable = UrlContext.GetUrlForAction<PayTrainerController>(x => x.TrainerReceipt(null),AreaName.Billing)+"/"+trainerPayment.EntityId+"?ParentId="+trainer.EntityId;
            return new CustomJsonResult(notification);
        }

        public ActionResult TrainerReceipt(ViewModel input)
        {
            var trainer = _repository.Query<User>(x => x.EntityId == input.ParentId)
                .FetchMany(x => x.TrainerPayments)
                .ThenFetchMany(x => x.TrainerPaymentSessionItems)
                .ThenFetch(x=>x.Appointment)    
                .ThenFetchMany(x=>x.Clients).ToList().FirstOrDefault();
            var payment = trainer.TrainerPayments.FirstOrDefault(x => x.EntityId == input.EntityId);
            var model = new TrainerReceiptViewModel
                                              {
                                                  Trainer = trainer,
                                                  TrainerPayment = payment
                                              };
            return View(model);
        }
    }

    public class TrainerReceiptViewModel : ViewModel
    {
        public User Trainer { get; set; }
        public TrainerPayment TrainerPayment { get; set; }
    }
}