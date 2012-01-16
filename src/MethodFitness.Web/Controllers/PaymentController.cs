using System;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Html;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class PaymentController:MFController
    {
        private readonly IRepository _repository;

        public PaymentController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult AddUpdate(ViewModel input)
        {
            var session = input.EntityId > 0 ? _repository.Find<Session>(input.EntityId) : new Session { StartDate = DateTime.Now };
            var model = new SessionViewModel
            {
                Item = session,
                Title = WebLocalizationKeys.CLIENT_INFORMATION.ToString(),
                DeleteUrl = UrlContext.GetUrlForAction<SessionController>(x => x.Delete(null))
            };
            return PartialView(model);
        }
    }
}