using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Reports.Controllers
{
    public class TimeSheetController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISelectListItemService _selectListItemService;

        public TimeSheetController(IRepository repository, ISelectListItemService selectListItemService)
        {
            _repository = repository;
            _selectListItemService = selectListItemService;
        }

        public ActionResult Display(ViewModel input)
        {
            var trainers = _repository.Query<User>(x => x.UserRoles.Any(r => r.Name == SecurityUserGroups.Trainer.ToString()));
            var startDate = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            var model = new TimeSheetViewModel
                            {
                                TrainerList = _selectListItemService.CreateList(trainers, x => x.FullNameFNF, x => x.EntityId,false),
                                StartDate = startDate,
                                EndDate = startDate.AddDays(6),
                                _Title = WebLocalizationKeys.TIME_SHEET.ToString(),
                                ReportUrl = "/Areas/Reports/Reports/TimeSheet.aspx"
                            };
            return View(model);
        }
    }

    public class TimeSheetViewModel : ViewModel
    {
        public User Trainer { get; set; }
        public IEnumerable<SelectListItem> TrainerList { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string ReportUrl { get; set; }
    }
}