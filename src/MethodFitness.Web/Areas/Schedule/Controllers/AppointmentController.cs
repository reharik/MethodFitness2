using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Components.Validator;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class AppointmentController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISelectListItemService _selectListItemService;
        private readonly ISaveEntityService _saveEntityService;

        public AppointmentController(IRepository repository,
            ISelectListItemService selectListItemService,
            ISaveEntityService saveEntityService)
        {
            _repository = repository;
            _selectListItemService = selectListItemService;
            _saveEntityService = saveEntityService;
        }

        public ActionResult AddUpdate(AddEditAppointmentViewModel input)
        {
            var appointment = input.EntityId > 0 ? _repository.Find<Appointment>(input.EntityId) : new Appointment();
            appointment.ScheduledDate = input.ScheduledDate.HasValue ? input.ScheduledDate.Value : appointment.ScheduledDate;
            appointment.ScheduledStartTime = input.StartTime.HasValue ? input.StartTime.Value : appointment.ScheduledStartTime;
            var locations = _selectListItemService.CreateList<Location>(x => x.Name, x => x.EntityId, true);
            var trainers = _repository.Query<User>(x=>x.UserRoles.Any(y=>y.Name=="Trainer"));
            var trainersList = _selectListItemService.CreateList(trainers,x => x.FullNameFNF, x => x.EntityId, true);
            var model = new AppointmentViewModel
            {
                LocationList = locations,
                TrainerList = trainersList,
                Appointment = appointment,
                Copy = input.Copy,
                Title = WebLocalizationKeys.APPOINTMENT_INFORMATION.ToString()
            };
            return PartialView("AddUpdate", model);
        }

        public ActionResult Display(ViewModel input)
        {
            var _event = _repository.Find<Appointment>(input.EntityId);
            var model = new AppointmentViewModel
            {
                Appointment = _event,
                AddUpdateUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.AddUpdate(null),AreaName.Schedule) + "/" + _event.EntityId,
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

        // getting the repo version of _event in action so I can tell if the _event was completed in past
        // maybe not so cool.  don't know
        private void mapToDomain(AppointmentViewModel model, Appointment appointment)
        {
            var appointmentModel = model.Appointment;
            var location = _repository.Find<Location>(appointmentModel.Location.EntityId);
            appointment.ScheduledDate = appointmentModel.ScheduledDate;
            appointment.ScheduledStartTime = DateTime.Parse(appointmentModel.ScheduledDate.Value.ToShortDateString() + " " + appointmentModel.ScheduledStartTime.Value.ToShortTimeString());
            if (appointmentModel.ScheduledEndTime.HasValue)
            {
                appointment.ScheduledEndTime = DateTime.Parse(appointmentModel.ScheduledDate.Value.ToShortDateString() + " " + appointmentModel.ScheduledEndTime.Value.ToShortTimeString());
            }

            appointment.Location = location;
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
    }

    public class AddEditAppointmentViewModel : ViewModel
    {
        public DateTime? StartTime { get; set; }
        public bool Copy { get; set; }
        public DateTime? ScheduledDate { get; set; }
    }
}