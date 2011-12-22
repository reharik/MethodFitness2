﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Components.Validator;
using MethodFitness.Core;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Localization;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;
using xVal.ServerSide;

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

        public AppointmentController(IRepository repository,
            ISelectListItemService selectListItemService,
            ISaveEntityService saveEntityService,
            ISessionContext sessionContext,
            IUserPermissionService userPermissionService,
            IUpdateCollectionService updateCollectionService)
        {
            _repository = repository;
            _selectListItemService = selectListItemService;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
            _userPermissionService = userPermissionService;
            _updateCollectionService = updateCollectionService;
        }

        public ActionResult AddUpdate(AddEditAppointmentViewModel input)
        {
            var appointment = input.EntityId > 0 ? _repository.Find<Appointment>(input.EntityId) : new Appointment();
            appointment.Date = input.ScheduledDate.HasValue ? input.ScheduledDate.Value : appointment.Date;
            appointment.StartTime = input.ScheduledStartTime.HasValue ? input.ScheduledStartTime.Value : appointment.StartTime;
            var locations = _selectListItemService.CreateList<Location>(x => x.Name, x => x.EntityId, true);
            var clients = _repository.FindAll<Client>();
            var availableClients = clients.Select(x => new TokenInputDto { id = x.EntityId, name = x.FullNameLNF});
            var selectedClients = appointment.Clients.Select(x => new TokenInputDto { id = x.EntityId, name = x.FullNameLNF });
            var model = new AppointmentViewModel
                            {
                                AvailableItems = availableClients,
                                SelectedItems = selectedClients,
                                LocationList = locations,
                                Appointment = appointment,
                                Copy = input.Copy,
                                Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString()
                            };
            model.Appointment.EntityId = input.Copy ? 0 : model.Appointment.EntityId;
            handelTime(model, appointment.StartTime.Value);
            handleTrainer(model);
            return PartialView("AddUpdate", model);
        }

        public void handelTime(AppointmentViewModel model, DateTime startTime)
        {
            model.sHour = startTime.Hour <= 12 ? startTime.Hour.ToString() : (startTime.Hour - 12).ToString();
            model.sMinutes = startTime.Minute.ToString();
            model.sAMPM = startTime.Hour > 12 ? "PM" : "AM";
            var endHour = (Int32.Parse(model.sHour) + 1);
            endHour = endHour > 12 ? endHour - 12 : endHour;
            model.eHour = endHour.ToString();
            model.eMinutes = model.sMinutes;
            model.eAMPM = model.eHour == "1" ? "PM" : model.sAMPM;

        }

        private void handleTrainer(AppointmentViewModel model)
        {
            if (_userPermissionService.IsAllowed("/Calendar/SetAppointmentForOthers"))
            {
                var trainers = _repository.Query<User>(x => x.UserRoles.Any(y => y.Name == "Trainer"));
                model.TrainerList = _selectListItemService.CreateList(trainers, x => x.FullNameFNF, x => x.EntityId, true);
            }else
            {
                var userId = _sessionContext.GetUserEntityId();
                var user = _repository.Find<User>(userId);
                model.TrainerName = user.FullNameFNF;
                model.Appointment.Trainer = user;
            }
        }

        public ActionResult Display(ViewModel input)
        {
            var _event = _repository.Find<Appointment>(input.EntityId);
            var model = new AppointmentViewModel
            {
                Appointment = _event,
                AddUpdateUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.AddUpdate(null),AreaName.Schedule) + "?EntityId=" + _event.EntityId,
                Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString()
            };
            return PartialView(model);
        }

        public ActionResult Delete(ViewModel input)
        {
            //TODO needs rule engine
            var appointment = _repository.Find<Appointment>(input.EntityId);
            _repository.HardDelete(appointment);
            _repository.UnitOfWork.Commit();
            return null;
        }

        public ActionResult Save(AppointmentViewModel input)
        {
            var _event = input.Appointment.EntityId > 0 ? _repository.Find<Appointment>(input.Appointment.EntityId) : new Appointment();
            mapToDomain(input, _event);
            Notification notification;
            if (!_event.Clients.Any())
            {
                notification = new Notification {Success = false};
                notification.Errors = new List<ErrorInfo> {new ErrorInfo(WebLocalizationKeys.CLIENTS.ToString(),WebLocalizationKeys.SELECT_AT_LEAST_ONE_CLIENT.ToString())};
                return Json(notification, JsonRequestBehavior.AllowGet);
            }
            var crudManager = _saveEntityService.ProcessSave(_event);
            notification = crudManager.Finish();
            return Json(notification, JsonRequestBehavior.AllowGet);
        }

        private void mapToDomain(AppointmentViewModel model, Appointment appointment)
        {
            var appointmentModel = model.Appointment;
            appointment.Date = appointmentModel.Date;
            appointment.StartTime = DateTime.Parse(appointmentModel.Date.Value.ToShortDateString() + " " + model.sHour+":"+model.sMinutes+" "+model.sAMPM);
            appointment.EndTime = DateTime.Parse(appointmentModel.Date.Value.ToShortDateString() + " " + model.eHour + ":" + model.eMinutes + " " + model.eAMPM);
            var trainer = _repository.Find<User>(appointmentModel.Trainer.EntityId);
            var location = _repository.Find<Location>(appointmentModel.Location.EntityId);
            appointment.Trainer = trainer;
            appointment.Location = location;
            appointment.Notes = appointmentModel.Notes;
            _updateCollectionService.UpdateFromCSV(appointment.Clients, model.ClientInput, appointment.AddClient, appointment.RemoveClient);

            return;
        }
    }

    public class AppointmentViewModel : ViewModel
    {
        public Appointment Appointment { get; set; }
        [ValidateNonEmpty]
        public bool Copy { get; set; }
        public string ClientInput { get; set; }
        public IEnumerable<TokenInputDto> AvailableItems { get; set; }
        public IEnumerable<TokenInputDto> SelectedItems { get; set; }
        public IEnumerable<SelectListItem> LocationList { get; set; }
        public IEnumerable<SelectListItem> TrainerList { get; set; }
        [ValueOf(typeof(Hours))]
        public string sHour { get; set; }
        [ValueOf(typeof(Minutes))]
        public string sMinutes { get; set; }
        [ValueOf(typeof(AMPM))]
        public string sAMPM { get; set; }
        [ValueOf(typeof(Hours))]
        public string eHour { get; set; }
        [ValueOf(typeof(Minutes))]
        public string eMinutes { get; set; }
        [ValueOf(typeof(AMPM))]
        public string eAMPM { get; set; }


        public string TrainerName { get; set; }
    }

    public class AddEditAppointmentViewModel : ViewModel
    {
        public DateTime? ScheduledStartTime { get; set; }
        public bool Copy { get; set; }
        public DateTime? ScheduledDate { get; set; }

        public bool AsAdmin { get; set; }
    }
}