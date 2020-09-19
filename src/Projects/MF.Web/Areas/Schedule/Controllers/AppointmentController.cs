using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.CustomAttributes;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Enumerations;
using CC.Core.Core.Html.Menu;
using CC.Core.Core.Services;
using CC.Core.Core.ValidationServices;
using CC.Core.DataValidation;
using CC.Core.Security.Interfaces;
using CC.Core.Utilities;
using MF.Core;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Config;
using MF.Web.Controllers;

namespace MF.Web.Areas.Schedule.Controllers
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
        private readonly IClientSessionService _clientSessionService;
        private readonly ILogger _logger;

        public AppointmentController(IRepository repository,
            ISelectListItemService selectListItemService,
            ISaveEntityService saveEntityService,
            ISessionContext sessionContext,
            IUserPermissionService userPermissionService,
            IUpdateCollectionService updateCollectionService,
            IAuthorizationService authorizationService,
            IClientSessionService clientSessionService,
            ILogger logger)
        {
            _repository = repository;
            _selectListItemService = selectListItemService;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
            _userPermissionService = userPermissionService;
            _updateCollectionService = updateCollectionService;
            _authorizationService = authorizationService;
            _clientSessionService = clientSessionService;
            _logger = logger;
        }

        public ActionResult AddUpdate_Template(AddEditAppointmentViewModel input)
        {
            return View("AddUpdate", new AppointmentViewModel());
        }

        public CustomJsonResult AddUpdate(AddEditAppointmentViewModel input)
        {
            var appointment = input.EntityId > 0 ? _repository.Find<Appointment>(input.EntityId) : new Appointment();
            appointment.Date = input.ScheduledDate.HasValue ? input.ScheduledDate.Value : appointment.Date;
            appointment.StartTime = input.ScheduledStartTime.HasValue ? input.ScheduledStartTime.Value : appointment.StartTime;
            var locations = _selectListItemService.CreateList<Location>(x => x.Name, x => x.EntityId, true);
            var userEntityId = _sessionContext.GetUserId();
            var trainer = _repository.Find<User>(userEntityId);
            IEnumerable<Client> clients = !this._authorizationService.IsAllowed(trainer, "/Clients/CanScheduleAllClients")
                    ? trainer.Clients.Where(x => !x.Archived)
                    : this._repository.Query<Client>(x => !x.Archived);
            var _availableClients = clients.OrderBy(x=>x.LastName).Select(x => new TokenInputDto { id = x.EntityId.ToString(), name = x.FullNameLNF});
            var selectedClients = appointment.Clients.Select(x => new TokenInputDto { id = x.EntityId.ToString(), name = x.FullNameLNF });

            if (input.Copy)
            {
                appointment = (Appointment)appointment.CloneSelf();
            }

            var model = Mapper.Map<Appointment, AppointmentViewModel>(appointment);
            model._StartTimeStringList = _selectListItemService.CreateList<Time>();
            model.ClientsDtos = new TokenInputViewModel { _availableItems = _availableClients, selectedItems = selectedClients };
            model._LocationEntityIdList = locations;
            model._saveUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.Save(null));
            model.Copy = input.Copy;
            model._Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString();
            model._AppointmentTypeList = _selectListItemService.CreateList<AppointmentType>(true);
            model.AppointmentType = appointment.AppointmentType ?? "Hour";
            model.StartTimeString = appointment.StartTime.Value.ToShortTimeString();
            model.EndTimeString = getEndTime(model.AppointmentType, appointment.StartTime.Value).ToShortTimeString();
            handleTrainer(model);
            return new CustomJsonResult(model);
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
                var trainers = _repository.Query<User>(x => !x.Archived && x.UserRoles.Any(y => y.Name == "Trainer"));
                model._TrainerEntityIdList = _selectListItemService.CreateList(trainers, x => x.FullNameFNF, x => x.EntityId, true);
            }else
            {
                var userId = _sessionContext.GetUserId();
                var trainer = _repository.Find<User>(userId);
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
            model.StartTimeString = appointment.StartTime.Value.ToShortTimeString();
            model.EndTimeString = appointment.EndTime.Value.ToShortTimeString();
            return new CustomJsonResult(model);
        }

        public ActionResult Delete(ViewModel input)
        {
            //TODO needs rule engine
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            var appointment = _repository.Find<Appointment>(input.EntityId);
            _logger.LogInfo("deleting appointmentId: {0}, date: {1}, trainer: {2}, clientIds: {3}, For: {4} "
                .ToFormat(appointment.EntityId, appointment.Date, appointment.Trainer.EntityId, String.Join(",",appointment.Clients.Select(x=>x.EntityId)),user.EntityId));
            if (appointment.StartTime < DateTime.Now.LocalizedDateTime("Eastern Standard Time").AddHours(6))
            {
                if (!_authorizationService.IsAllowed(user, "/Calendar/CanDeleteRetroactiveAppointments"))
                {
                    var notification = new Notification { Message = WebLocalizationKeys.YOU_CAN_NOT_DELETE_RETROACTIVELY.ToString() };
                    return Json(notification, JsonRequestBehavior.AllowGet);
                }
                _logger.LogInfo("appointmentId: {0}, being deleted happened in the past".ToFormat(appointment.EntityId));
                _clientSessionService.RestoreSessionsFromAppointment(appointment);
                // first save app to save the clients and sessions that have been restored
                _repository.Save(appointment);
            }
            _repository.HardDelete(appointment);
            _repository.UnitOfWork.Commit();
            return new CustomJsonResult(new Notification{Success = true});
        }

        public ActionResult Save(AppointmentViewModel input)
        {
            var appointment = input.EntityId > 0 ? _repository.Find<Appointment>(input.EntityId) : new Appointment();
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);

            var notification = validateAppointment(user, input);
            if (!notification.Success)
            {
                return Json(notification, JsonRequestBehavior.AllowGet);
            }
            if (appointment.Completed
                && appointment.StartTime < DateTime.Now.LocalizedDateTime("Eastern Standard Time"))
            {
                _logger.LogInfo("updating AppointmentId: {0}, that was already completed".ToFormat(appointment.EntityId));
                var newListOfClientIds = input.ClientsDtos.selectedItems.Select(x => Int32.Parse(x.id));
                _clientSessionService.SettleChangesToPastAppointment(newListOfClientIds, appointment, input.AppointmentType);
            }
            // map or remap values
            mapToDomain(input, appointment);
            // check here to see if session that is removed (or possibly client that is removed) is still on apt
            var crudManager = _saveEntityService.ProcessSave(appointment);
            var continuation = crudManager.Finish();
            return new CustomJsonResult(new Notification(continuation));
        }

        private Notification validateAppointment(User user, AppointmentViewModel input)
        {
            var startTime = DateTime.Parse(input.Date.ToShortDateString() + " " + input.StartTimeString);
            var endTime = DateTime.Parse(input.Date.ToShortDateString() + " " + input.EndTimeString);

            var notification = validateOverBooking(user, input, startTime, endTime);

            var convertTime = DateTime.Now.LocalizedDateTime("Eastern Standard Time");
            if (startTime < convertTime.AddHours(6) && !_authorizationService.IsAllowed(user, "/Calendar/CanEnterRetroactiveAppointments"))
            {
                notification.Success = false;
                notification.Message = CoreLocalizationKeys.YOU_CAN_NOT_CREATE_RETROACTIVE_APPOINTMENTS.ToString();
                return notification;
            }

            if (input.ClientsDtos==null || !input.ClientsDtos.selectedItems.Any())
            {
                notification = new Notification { Success = false };
                notification.Errors = new List<ErrorInfo> { new ErrorInfo(CoreLocalizationKeys.CLIENTS.ToString(), CoreLocalizationKeys.SELECT_AT_LEAST_ONE_CLIENT.ToString()) };
            }
            return notification;
        }

        private Notification validateOverBooking(User user, AppointmentViewModel input, DateTime startTime, DateTime endTime)
        {
            var notification = new Notification { Success = true };
            if (user.UserRoles.Exists(x => x.Name == "Administrator"))
            {
                return notification;
            }
            var surroundingApts = _repository.Query<Appointment>(x => x.StartTime < endTime
                && x.EndTime > startTime).Where(x => x.Location.EntityId == input.LocationEntityId).ToList();

            var map = new TimeMap();
            var length = 0;
            var slots = 1;
            switch(input.AppointmentType) {
                case "Hour": {
                    length = 4;
                    break;
                }
                case "Half Hour": {
                    length = 2;
                    break;
                }
                case "Pair": {
                    length = 4;
                    slots = 2;
                    break;
                }
            }
            _logger.LogWarn("length: " + length.ToString());
            _logger.LogWarn("slots: " + slots.ToString());
            surroundingApts.ForEach(x =>
            {
                for (int i = 0; i <= length; i++)
                {
                    var st = new DateTime(startTime.Ticks).AddMinutes(15 * i);
                    _logger.LogWarn("st: " + st.ToString());
                    _logger.LogWarn("x.startTime: " + x.StartTime.ToString());
                    _logger.LogWarn("x.EndTime: " + x.EndTime.ToString());
                    _logger.LogWarn("x.StartTime <= st && x.EndTime > st :" + (x.StartTime <= st && x.EndTime > st).ToString());

                    if (x.StartTime <= st && x.EndTime > st)
                    {
                        var time = 15 * (i + 1);
                        switch (time)
                        {
                            case 15:
                                {
                                    map._15 = map._15 + slots;
                                    break;
                                }
                            case 30:
                                {
                                    map._30 = map._30 + slots;
                                    break;
                                }
                            case 45:
                                {
                                    map._45 = map._45 + slots;
                                    break;
                                }
                            case 60:
                                {
                                    map._60 = map._60 + slots;
                                    break;
                                }
                        }
                    }
                }
            });
            _logger.LogWarn("map15: " + map._15);
            _logger.LogWarn("map30: " + map._30);
            _logger.LogWarn("map45: " + map._45);
            _logger.LogWarn("map60: " + map._60);

            var count = input.LocationEntityId == 2 ? 6 : 2;
            if(map._15>=count || map._30>=count || map._45>=count || map._60>=count) {
                notification.Success = false;
                notification.Message = CoreLocalizationKeys.LOCATION_HAS_NO_SPACE_AVAILABLE.ToString();
            }
                return notification;

        }

        private void mapToDomain(AppointmentViewModel model, Appointment appointment)
        {
            appointment.Date = model.Date;
            appointment.StartTime = DateTime.Parse(model.Date.ToShortDateString()+" "+model.StartTimeString);
            var endTime = getEndTime(model.AppointmentType, appointment.StartTime.Value);
            appointment.EndTime = DateTime.Parse(model.Date.ToShortDateString() + " " + endTime.ToShortTimeString()); 
            appointment.AppointmentType = model.AppointmentType;
            var trainer = _repository.Find<User>(model.TrainerEntityId);
            var location = _repository.Find<Location>(model.LocationEntityId);
            appointment.Trainer = trainer;
            appointment.Location = location;
            appointment.Notes = model.Notes;
            _updateCollectionService.Update(appointment.Clients.ToList(), model.ClientsDtos, appointment.AddClient, appointment.RemoveClient);
        }
    }

    public class AppointmentViewModel : ViewModel
    {
        [Required]
        public bool Copy { get; set; }
        [Required]
        public TokenInputViewModel ClientsDtos { get; set; }
        public IEnumerable<SelectListItem> _LocationEntityIdList { get; set; }
        public IEnumerable<SelectListItem> _TrainerEntityIdList { get; set; }
        public IEnumerable<SelectListItem> _StartTimeStringList { get; set; }
        public IEnumerable<SelectListItem> _AppointmentTypeList { get; set; }

        public string TrainerFullNameFNF { get; set; }
        [Required]
        public int LocationEntityId { get; set; }
        public string LocationName { get; set; }
        [Required]
        public int TrainerEntityId { get; set; }
        public string AppointmentType { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string StartTimeString { get; set; }
        [Required]
        public string EndTimeString { get; set; }
        [TextArea]
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

    public class TimeMap
    {
        public int _15 { get; set; }
        public int _30 { get; set; }
        public int _45 { get; set; }
        public int _60 { get; set; }
    }
}
