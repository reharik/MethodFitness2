using System.ComponentModel.DataAnnotations;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Services;
using MF.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MF.Web.Config;
using MF.Web.Controllers;
using MF.Core.Services;

namespace MF.Web.Areas.Reporting.Controllers
{
    public class ActivityController : AdminController
    {
        private readonly IRepository _repository;
        private readonly ISelectListItemService _selectListItemService;
        private readonly ISessionContext _sessionContext;

        public ActivityController(IRepository repository, ISelectListItemService selectListItemService,
            ISessionContext sessionContext
            )
        {
            this._repository = repository;
            this._selectListItemService = selectListItemService;
            this._sessionContext = sessionContext;

        }
        public ActionResult Display_Template(ViewModel input)
        {
            return this.View("Display", new ActivityViewModel());
        }

        public CustomJsonResult Display(ViewModel input)
        {
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            var managerRole = user.UserRoles.FirstOrDefault(x => x.Role.Name == "Manager");
            IEnumerable<Location> locations;
            if (managerRole != null)
            {
                locations = managerRole.Locations;
            }
            else
            {
                locations = _repository.FindAll<Location>();
            }


            var trainers = this._repository.Query<User>(x => x.UserRoles.Any(y => y.Role.Name == "Trainer"));
            var model = new ActivityViewModel
            {
                _TrainerList = this._selectListItemService.CreateList(trainers, x => x.FullNameFNF, x => x.EntityId, true),
                _ClientList = this._selectListItemService.CreateList<Client>(x => x.FullNameFNF, x => x.EntityId, true),
                _LocationList = this._selectListItemService.CreateList<Location>(locations, x => x.Name, x => x.EntityId, true),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                _Title = WebLocalizationKeys.PAYMENTS.ToString(),
                ReportUrl = "/Areas/Reporting/ReportViewer/Activity.aspx"
            };
            return new CustomJsonResult(model);
        }
    }

    public class ActivityViewModel : ViewModel
    {
        public IEnumerable<SelectListItem> _TrainerList { get; set; }
        public int Trainer { get; set; }
        public IEnumerable<SelectListItem> _LocationList { get; set; }
        public int Location { get; set; }
        public IEnumerable<SelectListItem> _ClientList { get; set; }
        public int Client { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string ReportUrl { get; set; }
    }
}
