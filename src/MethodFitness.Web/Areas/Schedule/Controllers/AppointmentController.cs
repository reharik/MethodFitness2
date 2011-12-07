using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Components.Validator;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Localization;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class AppointmentController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISelectListItemService _selectListItemService;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ISessionContext _sessionContext;

        public AppointmentController(IRepository repository,
            ISelectListItemService selectListItemService,
            ISaveEntityService saveEntityService,
            ISessionContext sessionContext )
        {
            _repository = repository;
            _selectListItemService = selectListItemService;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
        }

        public ActionResult AddUpdate(AddEditAppointmentViewModel input)
        {
            var appointment = input.EntityId > 0 ? _repository.Find<Appointment>(input.EntityId) : new Appointment();
            appointment.ScheduledDate = input.ScheduledDate.HasValue ? input.ScheduledDate.Value : appointment.ScheduledDate;
            appointment.ScheduledStartTime = input.ScheduledStartTime.HasValue ? input.ScheduledStartTime.Value : appointment.ScheduledStartTime;
            var locations = _selectListItemService.CreateList<Location>(x => x.Name, x => x.EntityId, true);
            var model = new AppointmentViewModel
                            {
                                LocationList = locations,
                                Appointment = appointment,
                                Copy = input.Copy,
                                Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString()
                            };
            model.Appointment.EntityId = input.Copy ? 0 : model.Appointment.EntityId;
            handelTime(model,input.ScheduledStartTime.Value);
            handleTrainer(model, input.AsAdmin);
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

        private void handleTrainer(AppointmentViewModel model, bool isAdmin)
        {
            if (isAdmin)
            {
                var trainers = _repository.Query<User>(x => x.UserRoles.Any(y => y.Name == "Trainer"));
                model.TrainerList = _selectListItemService.CreateList(trainers, x => x.FullNameFNF, x => x.EntityId, true);
                model.AsAdmin = true;
            }else
            {
                var userId = _sessionContext.GetUserEntityId();
                var user = _repository.Find<User>(userId);
                model.TrainerName = user.FullNameFNF;
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
            var crudManager = _saveEntityService.ProcessSave(_event);
            var notification = crudManager.Finish();
            return Json(notification, JsonRequestBehavior.AllowGet);
        }

        private void mapToDomain(AppointmentViewModel model, Appointment appointment)
        {
            var appointmentModel = model.Appointment;
            appointment.ScheduledDate = appointmentModel.ScheduledDate;
            appointment.ScheduledStartTime = DateTime.Parse(appointmentModel.ScheduledDate.Value.ToShortDateString() + " " + appointmentModel.ScheduledStartTime.Value.ToShortTimeString());
            if (appointmentModel.ScheduledEndTime.HasValue)
            {
                appointment.ScheduledEndTime = DateTime.Parse(appointmentModel.ScheduledDate.Value.ToShortDateString() + " " + appointmentModel.ScheduledEndTime.Value.ToShortTimeString());
            }
            var trainer = _repository.Find<User>(appointmentModel.Trainer.EntityId);
            var location = _repository.Find<Location>(appointmentModel.Location.EntityId);
            appointment.Trainer = trainer;
            appointment.Location = location;
            appointment.Client = appointmentModel.Client;
            appointment.Notes = appointmentModel.Notes;
            return;
        }
    }

    public class AppointmentViewModel : ViewModel
    {
        public Appointment Appointment { get; set; }
        [ValidateNonEmpty]
        public bool Copy { get; set; }
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

        public bool AsAdmin { get; set; }

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