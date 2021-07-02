using System.ComponentModel.DataAnnotations;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Services;
using MF.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using MF.Web.Config;
using MF.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MF.Web.Areas.Reporting.Controllers
{
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
            var trainers = this._repository.Query<User>(x => x.UserRoles.Any(y => y.Name == "Trainer"));

            var model = new DailyPaymentViewModel
                            {
                                _TrainerList = this._selectListItemService.CreateList(trainers, x => x.FullNameFNF, x => x.EntityId, true),
                                _ClientList = this._selectListItemService.CreateList<Client>(x => x.FullNameFNF, x => x.EntityId, true),
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
        public IEnumerable<SelectListItem> _TrainerList { get; set; }
        public int Trainer { get; set; }
        public IEnumerable<SelectListItem> _ClientList { get; set; }
        public int Client { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string ReportUrl { get; set; }
    }
}
