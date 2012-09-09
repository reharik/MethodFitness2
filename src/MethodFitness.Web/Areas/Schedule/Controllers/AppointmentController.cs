using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Castle.Components.Validator;
using MethodFitness.Core;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Localization;
using MethodFitness.Core.Services;
using MethodFitness.Security.Interfaces;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class AppointmentController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISelectListItemService _selectListItemService;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ISessionContext _sessionContext;
        private readonly IUserPermissionService _userPermissionService;
        private readonly IUpdateCollectionService _updateCollectionService;
        private readonly IAuthorizationService _authorizationService;

        public AppointmentController(IRepository repository,
            ISelectListItemService selectListItemService,
            ISaveEntityService saveEntityService,
            ISessionContext sessionContext,
            IUserPermissionService userPermissionService,
            IUpdateCollectionService updateCollectionService,
            IAuthorizationService authorizationService)
        {
            _repository = repository;
            _selectListItemService = selectListItemService;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
            _userPermissionService = userPermissionService;
            _updateCollectionService = updateCollectionService;
            _authorizationService = authorizationService;
        }

        public ActionResult AddUpdate_Template(AddEditAppointmentViewModel input)
        {
            return View("AddUpdate", new AppointmentViewModel());
        }

        public ActionResult AddUpdate(AddEditAppointmentViewModel input)
        {
            var appointment = input.EntityId > 0 ? _repository.Find<Appointment>(input.EntityId) : new Appointment();
            appointment.Date = input.ScheduledDate.HasValue ? input.ScheduledDate.Value : appointment.Date;
            appointment.StartTime = input.ScheduledStartTime.HasValue ? input.ScheduledStartTime.Value : appointment.StartTime;
            var locations = _selectListItemService.CreateList<Location>(x => x.Name, x => x.EntityId, true);
            var userEntityId = _sessionContext.GetUserId();
            dynamic trainer = _repository.Find<User>(userEntityId);
            IEnumerable<Client> clients;
            if(!_authorizationService.IsAllowed(trainer,"/Clients/CanScheduleAllClients"))
            {
                clients = trainer.Clients;
            }else
            {
                clients = _repository.FindAll<Client>();
            }
            var _availableClients = clients.OrderBy(x=>x.LastName).Select(x => new TokenInputDto { id = x.EntityId.ToString(), name = x.FullNameLNF});
            var selectedClients = appointment.Clients.Select(x => new TokenInputDto { id = x.EntityId.ToString(), name = x.FullNameLNF });

            if (input.Copy)
            {
                appointment = (Appointment)appointment.CloneSelf();
            }

            var model = Mapper.Map<Appointment, AppointmentViewModel>(appointment);
            model.ClientsDtos = new TokenInputViewModel { _availableItems = _availableClients, selectedItems = selectedClients };
            model._LocationEntityIdList = locations;
            model._saveUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.Save(null));
            model.Copy = input.Copy;
            model._Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString();
            model._AppointmentTypeList = _selectListItemService.CreateList<AppointmentType>(true);
            model.EndTime = getEndTime(model.AppointmentType, appointment.StartTime.Value);
            handleTrainer(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private DateTime getEndTime(string length, DateTime startTime)
        {
            switch (length)
            {
                case "Half Hour":
                    return startTime.AddMinutes(30);
                case "Hour":
                case "Pair":
                    return startTime.AddMinutes(60);
            }
            return startTime.AddMinutes(60);
        }

        private void handleTrainer(AppointmentViewModel model)
        {
            if (_userPermissionService.IsAllowed("/Calendar/SetAppointmentForOthers"))
            {
                var trainers = _repository.Query<Trainer>(x => x.UserRoles.Any(y => y.Name == "Trainer"));
                model._TrainerEntityIdList = _selectListItemService.CreateList(trainers, x => x.FullNameFNF, x => x.EntityId, true);
            }else
            {
                var userId = _sessionContext.GetUserId();
                var trainer = _repository.Find<Trainer>(userId);
                model.TrainerFullNameFNF = trainer.FullNameFNF;
                model.TrainerEntityId = trainer.EntityId;
            }
        }

        public ActionResult Display_Template(ViewModel input)
        {
            return View("Display", new AppointmentViewModel());
        }

        public ActionResult Display(ViewModel input)
        {
            var appointment = _repository.Find<Appointment>(input.EntityId);
            var model = Mapper.Map<Appointment, AppointmentViewModel>(appointment);
            model._Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString();
            model._clientItems = appointment.Clients.Select(x => x.FullNameFNF);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(ViewModel input)
        {
            //TODO needs rule engine
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            var appointment = _repository.Find<Appointment>(input.EntityId);
            if (appointment.StartTime < DateTime.Now && !_authorizationService.IsAllowed(user, "/Calendar/CanDeleteRetroactiveAppointments"))
            {
                var notification = new Notification{Message=WebLocalizationKeys.YOU_CAN_NOT_DELETE_RETROACTIVELY.ToString()};
                return Json(notification,JsonRequestBehavior.AllowGet);
            }
            appointment.RestoreSessionsToClientWhenDeleted();
            //first save app to save the clients and sessions that have been restored
            _repository.Save(appointment);
            _repository.HardDelete(appointment);
            _repository.UnitOfWork.Commit();
            return Json(new Notification{Success = true},JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(AppointmentViewModel input)
        {
            var appointment = input.EntityId > 0 ? _repository.Find<Appointment>(input.EntityId) : new Appointment();
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            mapToDomain(input, appointment);
            var notification = new Notification { Success = true };
            notification = appointment.CheckPermissions(user, _authorizationService, notification);
            notification = appointment.CheckForClients(notification);
            if(appointment.EntityId==0)
            {
                appointment.SetSessionsForClients();
            }
            if(!notification.Success)
            {
                return Json(notification, JsonRequestBehavior.AllowGet);
            }
            var crudManager = _saveEntityService.ProcessSave(appointment);
            notification = crudManager.Finish();
            return Json(notification, JsonRequestBehavior.AllowGet);
        }

        private void mapToDomain(AppointmentViewModel model, Appointment appointment)
        {
            appointment.Date = model.Date;
            appointment.StartTime = DateTime.Parse(model.Date.ToShortDateString()+" "+model.StartTimeString);
            var endTime = getEndTime(model.AppointmentType, appointment.StartTime.Value);
            appointment.EndTime = DateTime.Parse(model.Date.ToShortDateString() + " " + endTime.ToShortTimeString()); 
            appointment.AppointmentType = model.AppointmentType;
            var trainer = _repository.Find<Trainer>(model.TrainerEntityId);
            var location = _repository.Find<Location>(model.LocationEntityId);
            appointment.Trainer = trainer;
            appointment.Location = location;
            appointment.Notes = model.Notes;
            _updateCollectionService.Update(appointment.Clients, model.ClientsDtos, appointment.AddClient, appointment.RemoveClient);

            
        }
    }

    public class AppointmentViewModel : ViewModel
    {
        [ValidateNonEmpty]
        public bool Copy { get; set; }
        public TokenInputViewModel ClientsDtos { get; set; }
        public IEnumerable<SelectListItem> _LocationEntityIdList { get; set; }
        public IEnumerable<SelectListItem> _TrainerEntityIdList { get; set; }
        public IEnumerable<SelectListItem> _sHourList { get; set; }
        public IEnumerable<SelectListItem> _sMinutesList { get; set; }
        public IEnumerable<SelectListItem> _sAMPMList { get; set; }
        public IEnumerable<SelectListItem> _AppointmentTypeList { get; set; }

        public string TrainerFullNameFNF { get; set; }
        public int LocationEntityId { get; set; }
        public string LocationName { get; set; }
        public int TrainerEntityId { get; set; }
        public string AppointmentType { get; set; }
        [ValidateNonEmpty]
        public DateTime Date { get; set; }
        [ValidateNonEmpty]
        public DateTime StartTime { get; set; }
        public string StartTimeString { get; set; }
        [ValidateNonEmpty]
        public DateTime EndTime { get; set; }
        public string EndTimeString { get; set; }
        
        public string Notes { get; set; }

        public IEnumerable<string> _clientItems { get; set; }
    }

    public class AddEditAppointmentViewModel : ViewModel
    {
        public DateTime? ScheduledStartTime { get; set; }
        public bool Copy { get; set; }
        public DateTime? ScheduledDate { get; set; }

        public bool AsAdmin { get; set; }
    }
}