using System;
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
using Rhino.Security.Interfaces;
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

        public ActionResult AddUpdate(AddEditAppointmentViewModel input)
        {
            var appointment = input.EntityId > 0 ? _repository.Find<Appointment>(input.EntityId) : new Appointment();
            appointment.Date = input.ScheduledDate.HasValue ? input.ScheduledDate.Value : appointment.Date;
            appointment.StartTime = input.ScheduledStartTime.HasValue ? input.ScheduledStartTime.Value : appointment.StartTime;
            var locations = _selectListItemService.CreateList<Location>(x => x.Name, x => x.EntityId, true);
            var userEntityId = _sessionContext.GetUserEntityId();
            var trainer = _repository.Find<Trainer>(userEntityId);
            IEnumerable<Client> clients;
            if(!_authorizationService.IsAllowed(trainer,"/Clients/CanScheduleAllClients"))
            {
                clients = trainer.Clients;
            }else
            {
                clients = _repository.FindAll<Client>();
            }
            var availableClients = clients.Select(x => new TokenInputDto { id = x.EntityId, name = x.FullNameLNF});
            var selectedClients = appointment.Clients.Select(x => new TokenInputDto { id = x.EntityId, name = x.FullNameLNF });
            var model = new AppointmentViewModel{
                            
                                AvailableItems = availableClients,
                                SelectedItems = selectedClients,
                                LocationList = locations,
                                Appointment = appointment,
                                Copy = input.Copy,
                                Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString()
                            };
            if(input.Copy)
            {
                model.Appointment = (Appointment) appointment.CloneSelf();
            }
            handelTime(model, appointment.StartTime.Value);
            handleTrainer(model);
            return PartialView("AddUpdate", model);
        }

        public void handelTime(AppointmentViewModel model, DateTime startTime)
        {
            getStartTime(model,startTime);
            model.Appointment.EndTime = getEndTime(model.Appointment.Length, startTime);
        }

        private void getStartTime(AppointmentViewModel model, DateTime startTime)
        {
            model.sHour = startTime.Hour <= 12 ? startTime.Hour.ToString() : (startTime.Hour - 12).ToString();
            model.sMinutes = startTime.Minute.ToString();
            model.sAMPM = startTime.Hour >= 12 ? "PM" : "AM";
        }
        private DateTime getEndTime(string length, DateTime startTime)
        {
            switch (length)
            {
                case "Half Hour":
                    return startTime.AddMinutes(30);
                case "Hour":
                    return startTime.AddMinutes(60);
                case "Hour And A Half":
                    return startTime.AddMinutes(90);
            }
            return startTime.AddMinutes(60);
        }

        private void handleTrainer(AppointmentViewModel model)
        {
            if (_userPermissionService.IsAllowed("/Calendar/SetAppointmentForOthers"))
            {
                var trainers = _repository.Query<Trainer>(x => x.UserRoles.Any(y => y.Name == "Trainer"));
                model.TrainerList = _selectListItemService.CreateList(trainers, x => x.FullNameFNF, x => x.EntityId, true);
            }else
            {
                var userId = _sessionContext.GetUserEntityId();
                var trainer = _repository.Find<Trainer>(userId);
                model.TrainerName = trainer.FullNameFNF;
                model.Appointment.Trainer = trainer;
            }
        }

        public ActionResult Display(ViewModel input)
        {
            var _event = _repository.Find<Appointment>(input.EntityId);
            var model = new AppointmentViewModel
            {
                Appointment = _event,
                Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString()
            };
            return PartialView(model);
        }

        public ActionResult Delete(ViewModel input)
        {
            //TODO needs rule engine
            var userEntityId = _sessionContext.GetUserEntityId();
            var user = _repository.Find<User>(userEntityId);
            var appointment = _repository.Find<Appointment>(input.EntityId);
            if (appointment.StartTime < DateTime.Now && !_authorizationService.IsAllowed(user, "/Calendar/CanDeleteRetroactiveAppointments"))
            {
                var notification = new Notification{Message=WebLocalizationKeys.YOU_CAN_NOT_DELETE_RETROACTIVELY.ToString()};
                return Json(notification,JsonRequestBehavior.AllowGet);
            }
            appointment.RestoreSessionsWhenDeleted();
            //first save app to save the clients and sessions that have been restored
            _repository.Save(appointment);
            _repository.HardDelete(appointment);
            _repository.UnitOfWork.Commit();
            return Json(new Notification{Success = true},JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(AppointmentViewModel input)
        {
            var appointment = input.Appointment.EntityId > 0 ? _repository.Find<Appointment>(input.Appointment.EntityId) : new Appointment();
            var userEntityId = _sessionContext.GetUserEntityId();
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
            var appointmentModel = model.Appointment;
            appointment.Date = appointmentModel.Date;
            appointment.StartTime = DateTime.Parse(appointmentModel.Date.Value.ToShortDateString() + " " + model.sHour+":"+model.sMinutes+" "+model.sAMPM);
            appointment.EndTime = getEndTime(model.Appointment.Length, appointment.StartTime.Value);
            appointment.Length =model.Appointment.Length;
            var trainer = _repository.Find<Trainer>(appointmentModel.Trainer.EntityId);
            var location = _repository.Find<Location>(appointmentModel.Location.EntityId);
            appointment.Trainer = trainer;
            appointment.Location = location;
            appointment.Notes = appointmentModel.Notes;
            _updateCollectionService.UpdateFromCSV(appointment.Clients, model.ClientInput, appointment.AddClient, appointment.RemoveClient);
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