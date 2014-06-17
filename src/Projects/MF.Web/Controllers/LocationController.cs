using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using CC.Core;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Enumerations;
using CC.Core.Html;
using CC.Core.Localization;
using CC.Core.Services;
using CC.Core.ValidationServices;
using CC.Utility;
using Castle.Components.Validator;
using MF.Core.Domain;
using MF.Core.Rules;
using MF.Web.Areas.Schedule.Models.BulkAction;
using MF.Web.Config;
using MethodFitness.Web;
using StructureMap;

namespace MF.Web.Controllers
{
    public class LocationController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ISelectListItemService _selectListItemService;

        public LocationController(IRepository repository,
            ISaveEntityService saveEntityService,
            ISelectListItemService selectListItemService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _selectListItemService = selectListItemService;
        }

        public ActionResult AddUpdate_Template(ViewModel input)
        {
            return View("AddUpdate", new LocationViewModel());
        }

        public ActionResult AddUpdate(ViewModel input)
        {
            Location location = input.EntityId > 0 ? _repository.Find<Location>(input.EntityId) : new Location();
            var model = Mapper.Map<Location, LocationViewModel>(location);

            model._Title = WebLocalizationKeys.CLIENT_INFORMATION.ToString();
            model._deleteUrl = UrlContext.GetUrlForAction<LocationController>(x => x.Delete(null));
            model._saveUrl = UrlContext.GetUrlForAction<LocationController>(x => x.Save(null));
            model._StateList = _selectListItemService.CreateList<State>();
            return new CustomJsonResult(model);
        }

        public ActionResult Delete(ViewModel input)
        {
            var location = _repository.Find<Location>(input.EntityId);
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeleteLocationRules");
            var validationManager = rulesEngineBase.ExecuteRules(location);
            if (validationManager.GetLastValidationReport().Success)
            {
                _repository.Delete(location);
            }
            var notification = validationManager.Finish();
            return new CustomJsonResult(notification);

        }

        public ActionResult DeleteMultiple(BulkActionViewModel input)
        {
            var rulesEngineBase = ObjectFactory.Container.GetInstance<RulesEngineBase>("DeleteLocationRules");
            IValidationManager validationManager = new ValidationManager(_repository);
            input.EntityIds.ForEachItem(x =>
            {
                var location = _repository.Find<Location>(x);
                validationManager = rulesEngineBase.ExecuteRules(location, validationManager);
                var report = validationManager.GetLastValidationReport();
                if (report.Success)
                {
                    report.SuccessAction = a => _repository.Delete((Location)a);
                }
            });
            var notification = validationManager.FinishWithAction();
            return new CustomJsonResult(notification);
        }

        public ActionResult Save(LocationViewModel input)
        {
            Location location = input.EntityId > 0 ? _repository.Find<Location>(input.EntityId) : new Location();
            location = mapToDomain(input, location);
            var crudManager = _saveEntityService.ProcessSave(location);

            var notification = crudManager.Finish();
            return new CustomJsonResult(notification);
        }

        private Location mapToDomain(LocationViewModel locationModel, Location location)
        {
            location.Address1 = locationModel.Address1;
            location.Address2 = locationModel.Address2;
            location.Name = locationModel.Name;
            location.City = locationModel.City;
            location.State = locationModel.State;
            location.Zip = locationModel.Zip;
            return location;
        }
    }

    public class LocationViewModel : ViewModel
    {
        public string _deleteUrl { get; set; }
        [ValidateNonEmpty]
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [ValueOf(typeof(State))]
        public string State { get; set; }
        public IEnumerable<SelectListItem> _StateList { get; set; }
        public string Zip { get; set; }
    }
}


