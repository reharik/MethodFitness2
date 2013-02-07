namespace MethodFitness.Web.Areas.Reporting.Controllers
{
    using System;
    using System.Web.Mvc;

    using CC.Core.CoreViewModelAndDTOs;
    using CC.Core.DomainTools;
    using CC.Core.Services;

    using Castle.Components.Validator;

    using MethodFitness.Web.Config;
    using MethodFitness.Web.Controllers;

    public class DailyPaymentsController : AdminController
    {
        private readonly IRepository _repository;
        private readonly ISelectListItemService _selectListItemService;

        public DailyPaymentsController(IRepository repository, ISelectListItemService selectListItemService)
        {
            this._repository = repository;
            this._selectListItemService = selectListItemService;
        }
        public ActionResult Display_Template(ViewModel input)
        {
            return this.View("Display", new DailyPaymentViewModel());
        }

        public CustomJsonResult Display(ViewModel input)
        {
            var model = new DailyPaymentViewModel
                            {
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now,
                                _Title = WebLocalizationKeys.DAILY_PAYMENTS.ToString(),
                                ReportUrl = "/Areas/Reporting/ReportViewer/DailyPayments.aspx"
                            };
            return new CustomJsonResult(model);
        }
    }

    public class DailyPaymentViewModel : ViewModel
    {
        [ValidateNonEmpty]
        public DateTime StartDate { get; set; }
        [ValidateNonEmpty]
        public DateTime EndDate { get; set; }
        public string ReportUrl { get; set; }
    }
}
