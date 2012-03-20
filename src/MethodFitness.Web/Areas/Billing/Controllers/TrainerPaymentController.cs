using System;
using System.Linq;
using System.Web.Mvc;
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
    public class TrainerPaymentController : MFController
    {
        private readonly IRepository _repository;

        public TrainerPaymentController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Display(ViewModel input)
        {
            var trainerPayment = _repository.Find<TrainerPayment>(input.EntityId);
            var model = new TrainerPaymentViewModel
            {
                Item = trainerPayment,
                Title = WebLocalizationKeys.PAYMENT_INFORMATION.ToString(),
            };
            return View(model);
        }
    }

    public class TrainerPaymentViewModel : ViewModel
    {
        public TrainerPayment Item { get; set; }
    }

}