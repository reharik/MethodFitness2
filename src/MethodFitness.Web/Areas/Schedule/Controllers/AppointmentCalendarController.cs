using System;
using System.Collections.Generic;
using System.Web.Mvc;
using KnowYourTurf.Core;
using MethodFitness.Core;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class AppointmentCalendarController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ISessionContext _sessionContext;

        public AppointmentCalendarController(IRepository repository,
            ISaveEntityService saveEntityService,
            ISessionContext sessionContext)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
        }

        public ActionResult AppointmentCalendar()
        {
            var model = new CalendarViewModel
            {
                CalendarDefinition = new CalendarDefinition
                {
                    Url = UrlContext.GetUrlForAction<AppointmentCalendarController>(x => x.Events(null), AreaName.Schedule),
                    AddEditUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.AddUpdate(null), AreaName.Schedule),
                    DisplayUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.Display(null), AreaName.Schedule),
                    EventChangedUrl = UrlContext.GetUrlForAction<AppointmentCalendarController>(x => x.EventChanged(null), AreaName.Schedule)
                }
            };
            return View(model);
        }

        public JsonResult EventChanged(AppointmentChangedViewModel input)
        {
            var appointment = _repository.Find<Appointment>(input.EntityId);
            appointment.ScheduledDate = input.ScheduledDate;
            appointment.ScheduledEndTime = input.EndTime;
            appointment.ScheduledStartTime = input.StartTime;
            var crudManager = _saveEntityService.ProcessSave(appointment);
            var notification = crudManager.Finish();
            return Json(notification, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Events(GetEventsViewModel input)
        {
            var events = new List<CalendarEvent>();
            var startDateTime = DateTimeUtilities.ConvertFromUnixTimestamp(input.start);
            var endDateTime = DateTimeUtilities.ConvertFromUnixTimestamp(input.end);
            var appointments = _repository.Query<Appointment>(x => x.ScheduledDate >= startDateTime && x.ScheduledDate <= endDateTime);
            appointments.Each(x => GetValue(x, events) );
            return Json(events, JsonRequestBehavior.AllowGet);
        }

        private void GetValue(Appointment x, List<CalendarEvent> events)
        {
            // create some operations here and grant to usergroups
            var userId = _sessionContext.GetUserEntityId();
            var calendarEvent = new CalendarEvent
                                    {
                                        EntityId = x.EntityId,
                                        title = x.Location.Name,
                                        start = x.ScheduledStartTime.ToString(),
                                        end = x.ScheduledEndTime.ToString(),
                                        color = x.Trainer.Color
                                    };
            events.Add(calendarEvent);
        }
    }

    

    public class AppointmentChangedViewModel : ViewModel
    {
        public DateTime? ScheduledDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }



    }
}