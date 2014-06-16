using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using MF.Core.Domain;
using MF.Web.Controllers;

namespace MF.Web.Areas.Billing.Controllers
{
    public class TrainerPaymentController : MFController
    {
        private readonly IRepository _repository;

        public TrainerPaymentController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Display(ViewModel input)
        {
//            var trainerPayment = _repository.Find<TrainerPayment>(input.EntityId);
//            var model = new TrainerPaymentViewModel
//            {
//                Item = trainerPayment,
//                _Title = WebLocalizationKeys.PAYMENT_INFORMATION.ToString(),
//            };
//            return View(model);
            return null;
        }
    }

    public class TrainerPaymentViewModel : ViewModel
    {
        public TrainerPayment Item { get; set; }
    }

}