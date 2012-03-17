using System.Linq;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Billing.Controllers
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

        public ActionResult PayTrainer(PayTrainerViewModel input)
        {
            Notification notification;
            var trainer = _repository.Find<User>(input.EntityId);
            var trainerPayment = trainer.PayTrainer(input.PaymentDetailsDto);
            if(trainerPayment==null)
            {
                notification = new Notification {Success = false, Message = WebLocalizationKeys.YOU_MUST_SELECT_AT_ONE_SESSION.ToString()};
                return Json(notification, JsonRequestBehavior.AllowGet);
            }
            var crudManager = _saveEntityService.ProcessSave(trainer);
            notification = crudManager.Finish();
            notification.Variable = UrlContext.GetUrlForAction<PayTrainerController>(x => x.TrainerReceipt(null),AreaName.Billing)+"/"+trainerPayment.EntityId+"?ParentId="+trainer.EntityId;
            return Json(notification, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrainerReceipt(ViewModel input)
        {
            var trainer = _repository.Find<User>(input.ParentId);
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