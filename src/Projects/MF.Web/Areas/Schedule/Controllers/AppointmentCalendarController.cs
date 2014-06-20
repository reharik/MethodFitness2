using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CC.Core;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using CC.Core.ValidationServices;
using CC.Security.Interfaces;
using CC.Utility;
using MF.Core;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Config;
using MF.Web.Controllers;
using MF.Web.Models;
using MF.Web;
using NHibernate.Linq;

namespace MF.Web.Areas.Schedule.Controllers
{
    public class AppointmentCalendarController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ISessionContext _sessionContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISelectListItemService _selectListItemService;

        public AppointmentCalendarController(IRepository repository,
            ISaveEntityService saveEntityService,
            ISessionContext sessionContext,
            IAuthorizationService authorizationService,
            ISelectListItemService selectListItemService
            )
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _sessionContext = sessionContext;
            _authorizationService = authorizationService;
            _selectListItemService = selectListItemService;
        }

        public ActionResult AppointmentCalendar_Template(CalendarViewModel input)
        {
            return View("AppointmentCalendar", new CalendarViewModel());
        }

        public ActionResult AppointmentCalendar(CalendarViewModel input)
        {
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            var locations = _selectListItemService.CreateList<Location>(x=>x.Name,x=>x.EntityId,false).ToList();
            locations.Insert(0,new SelectListItem{Text=WebLocalizationKeys.ALL.ToString(),Value = "0"});
            var trainersDto = new List<TrainerLegendDto>();
            if(user.UserRoles.Any(x=>x.Name==UserType.Administrator.ToString()))
            {
                var trainers = _repository.Query<User>(x => !x.Archived && x.UserRoles.Any(y => y.Name == UserType.Trainer.ToString()));
                trainersDto = trainers.Select(x=> new TrainerLegendDto {Name=processTrainerName(x),
                    Color=x.Color,
                EntityId = x.EntityId}).ToList();
            }

            var model = new CalendarViewModel
            {
                Trainers = trainersDto,
                _LocationList = locations,
//                CalendarUrl = UrlContext.GetUrlForAction<AppointmentCalendarController>(x=>x.AppointmentCalendar(null)),
                CalendarDefinition = new CalendarDefinition
                {
                    Url = UrlContext.GetUrlForAction<AppointmentCalendarController>(x => x.Events(null), AreaName.Schedule),
                    AddUpdateUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.AddUpdate(null), AreaName.Schedule),
                    DisplayUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.Display(null), AreaName.Schedule),
                    DeleteUrl = UrlContext.GetUrlForAction<AppointmentController>(x => x.Delete(null), AreaName.Schedule),
                    AddUpdateRoute = "event",
                    DisplayRoute = "eventdisplay",
                    EventChangedUrl = UrlContext.GetUrlForAction<AppointmentCalendarController>(x => x.EventChanged(null), AreaName.Schedule),
                    CanEditPastAppointments = _authorizationService.IsAllowed(user, "/Calendar/CanEditPastAppointments"),
                    CanEnterRetroactiveAppointments = _authorizationService.IsAllowed(user, "/Calendar/CanEnterRetroactiveAppointments"),
                    CanSeeOthersAppointments = _authorizationService.IsAllowed(user, "/Calendar/CanSeeOthersAppointments"),
                    TrainerId = user.EntityId
                }
            };
            return new CustomJsonResult(model);
        }
        public string processTrainerName(User trainer)
        {
            var name = trainer.LastName + ", " + trainer.FirstName[0];
            if(name.Length>12)
            {
                name = name.Substring(0, 12);
            }
            return name;
        }

        public CustomJsonResult EventChanged(AppointmentChangedViewModel input)
        {
            var appointment = _repository.Find<Appointment>(input.EntityId);
            appointment.Date = input.ScheduledDate;
            appointment.EndTime = input.EndTime;
            appointment.StartTime = input.StartTime;
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            var notification = validateAppointment(user, input);
            if (!notification.Success)
            {
                return new CustomJsonResult(notification);
            }
            var crudManager = _saveEntityService.ProcessSave(appointment);
            var continuation = crudManager.Finish();
            return new CustomJsonResult(new Notification(continuation));
        }

        private Notification validateAppointment(User user, AppointmentChangedViewModel input)
        {
            var notification = new Notification { Success = true };
            // nice to pull this off user
            var currentTime = DateTime.Now.LocalizedDateTime("Eastern Standard Time");
            if (input.StartTime < currentTime && !_authorizationService.IsAllowed(user, "/Calendar/CanEnterRetroactiveAppointments"))
            {
                notification.Success = false;
                notification.Message = CoreLocalizationKeys.YOU_CAN_NOT_CREATE_RETROACTIVE_APPOINTMENTS.ToString();
                return notification;
            }
            return notification;
        }

        public CustomJsonResult Events(GetEventsViewModel input)
        {
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            var canSeeOthers = _authorizationService.IsAllowed(user, "/Calendar/CanSeeOthersAppointments");
            var events = new List<CalendarEvent>();
            var startDateTime = DateTimeUtilities.ConvertFromUnixTimestamp(input.start);
            var endDateTime = DateTimeUtilities.ConvertFromUnixTimestamp(input.end);
            var appointments = input.Loc<=0
                ? _repository.Query<Appointment>(x => x.StartTime >= startDateTime && x.Date <= endDateTime).Fetch(x => x.Clients).Fetch(x => x.Trainer).AsEnumerable()
                : _repository.Query<Appointment>(x => x.StartTime >= startDateTime 
                                                && x.Date <= endDateTime
                                                && x.Location.EntityId == input.Loc).Fetch(x => x.Clients).Fetch(x => x.Trainer).AsEnumerable();
            if (input.TrainerIds.IsNotEmpty())
            {
                //This is necessary because when in trainer view the ledgend is not shown and returns "0"
                if (input.TrainerIds == "NONE") { appointments = new List<Appointment>(); }
                else
                {
                    IEnumerable<int> ids = input.TrainerIds.Split(',').Select(Int32.Parse).ToList();
                    appointments = appointments.Where(x => ids.Contains(x.Trainer.EntityId));
                }
            }
            appointments.ForEachItem(x => GetValue(x, events, user, canSeeOthers));
            return new CustomJsonResult(events);
        }

        private void GetValue(Appointment x, List<CalendarEvent> events, User user, bool canSeeOthers)
        {
            var calendarEvent = new CalendarEvent
                                    {
                                        EntityId = x.EntityId,
                                        start = x.StartTime.ToString(),
                                        end = x.EndTime.ToString(),
                                        color = x.Trainer.Color,
                                        trainerId = x.Trainer.EntityId
                                    };
            if(x.Clients.Count()>1)
            {
                calendarEvent.title = x.Location.Name + ": Multiple";
            }else if(x.Clients.Count()==1)
            {
                calendarEvent.title = x.Location.Name + ": " +x.Clients.FirstOrDefault().FullNameLNF;
            }

            if (x.Trainer != user && !canSeeOthers)
            {
                return;
                calendarEvent.color = "#fffff";
                calendarEvent.title = string.Empty;
                calendarEvent.className = "hiddenEvent";
            }
            events.Add(calendarEvent);
        }
    }

    public class TrainerLegendDto   
    {
        public string Name { get; set; }

        public string Color { get; set; }

        public int EntityId { get; set; }
    }

    public class AppointmentPermissionsDto
    {
        public bool CanSeeOthers { get; set; }
        public bool CanEditOthers { get; set; }
        public bool CanEnterRetroactive { get; set; }
        public bool CanEditRetroactive { get; set; }
    }

    public class AppointmentChangedViewModel : ViewModel
    {
        public DateTime? ScheduledDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }



    }
}