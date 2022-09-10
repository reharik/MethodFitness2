using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Enumerations;
using CC.Core.Core.Html;
using CC.Core.Core.Html.Menu;
using CC.Core.Core.Services;
using CC.Core.Core.ValidationServices;
using CC.Core.Security.Interfaces;
using CC.Core.Utilities;
using MF.Core;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Config;
using MF.Web.Controllers;
using MF.Web.Models;
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
            var locations = _selectListItemService.CreateList<Location>(x => x.Name, x => x.EntityId, false).ToList();
            locations.Insert(0, new SelectListItem { Text = WebLocalizationKeys.ALL.ToString(), Value = "0" });
            var trainersDto = new List<TrainerLegendDto>();
            if (user.UserRoles.Any(x => x.Role.Name == UserType.Administrator.ToString()))
            {
                var trainers = _repository.Query<User>(x => !x.Archived && x.UserRoles.Any(y => y.Role.Name == UserType.Trainer.ToString()));
                trainersDto = trainers.Select(x => new TrainerLegendDto
                {
                    Name = processTrainerName(x),
                    Color = x.Color,
                    EntityId = x.EntityId
                }).ToList();
            }
            var managerRole = user.UserRoles.FirstOrDefault(x => x.Role.Name == "Manager");
            var canSeeOthersAppointments = user.UserRoles.FirstOrDefault(x => x.Role.Name == "Administrator") !=null
                || managerRole != null;
            var locationsAvailableToManager = managerRole != null ? managerRole.LocationId.EntityId : -1;
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
                    CanEditOthersAppointments = _authorizationService.IsAllowed(user, "/Calendar/CanEditOthersAppointments"),
                    CanEnterRetroactiveAppointments = _authorizationService.IsAllowed(user, "/Calendar/CanEnterRetroactiveAppointments"),
                    CanSeeOthersAppointments = _authorizationService.IsAllowed(user, "/Calendar/CanSeeOthersAppointments"),
                    locationsAvailableToManager= locationsAvailableToManager,
                    TrainerId = user.EntityId
                }
            };
            return new CustomJsonResult(model);
        }
        public string processTrainerName(User trainer)
        {
            var name = trainer.LastName + ", " + trainer.FirstName[0];
            if (name.Length > 12)
            {
                name = name.Substring(0, 12);
            }
            return name;
        }

        public CustomJsonResult EventChanged(AppointmentChangedViewModel input)
        {
            var appointment = _repository.Find<Appointment>(input.EntityId);
            var originalStartTime = appointment.StartTime;
            var originalStartDate = appointment.Date;
            appointment.Date = input.ScheduledDate;
            appointment.EndTime = input.EndTime;
            appointment.StartTime = input.StartTime;
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            var notification = validateAppointment(user, input, originalStartTime, originalStartDate);
            if (!notification.Success)
            {
                return new CustomJsonResult(notification);
            }
            var crudManager = _saveEntityService.ProcessSave(appointment);
            var continuation = crudManager.Finish();
            return new CustomJsonResult(new Notification(continuation));
        }

        private Notification validateAppointment(User user, AppointmentChangedViewModel input, DateTime? ost, DateTime? osd)
        {
            var notification = new Notification { Success = true };
            // nice to pull this off user
            var currentTime = DateTime.Now.LocalizedDateTime("Eastern Standard Time");
            var original = new DateTime(osd.Value.Year, osd.Value.Month, osd.Value.Day, ost.Value.Hour, ost.Value.Minute, 0);
            if ((original < currentTime.AddHours(8) || input.StartTime < currentTime.AddHours(8))
                && !_authorizationService.IsAllowed(user, "/Calendar/CanEnterRetroactiveAppointments"))
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
            var managerRole = user.UserRoles.FirstOrDefault(x => x.Role.Name == "Manager");
            var adminRole = user.UserRoles.FirstOrDefault(x => x.Role.Name == "Administrator");
            var locationsAvailableToView = managerRole != null 
                ? managerRole.LocationId.EntityId 
                : adminRole != null
                ? 0 //all locations
                : -1; // no locations

            var events = new List<CalendarEvent>();
            var startDateTime = DateTimeUtilities.ConvertFromUnixTimestamp(input.start);
            var endDateTime = DateTimeUtilities.ConvertFromUnixTimestamp(input.end);
            var appointments = input.Loc <= 0
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
                    appointments = appointments.Where(x => ids.Contains(x.Trainer.EntityId) || x.Trainer.Archived);
                }
            }
            BlockedSlotsByLocation blockingSlots = CalculateBlockedSlots(appointments, user);

            if (!user.UserRoles.Any(x => x.Role.Name == "Administrator"))
            {
                CreateBlockingEvents(blockingSlots, events, locationsAvailableToView);
            }
            appointments.ForEachItem(x => GetValue(x, events, user, canSeeOthers, locationsAvailableToView));
            var payload = new CalendarEventPayload
            {
                Events = events,
                BlockedSlotsByLocation = blockingSlots
            };
            return new CustomJsonResult(payload);
        }

        private BlockedSlotsByLocation CalculateBlockedSlots(IEnumerable<Appointment> appts, User user)
        {
            var blocked = new BlockedSlotsByLocation();
            if (user.UserRoles.Any(x => x.Role.Name == "Administrator"))
            {
                return blocked;
            }
            
            var location2CutOff = 4;
            var location1CutOff = 2;

            blocked.Location = new List<LocationBlockedSlots>();

            appts.Where(x => x.Location.EntityId <= 2
                    && x.StartTime > DateTime.Now.AddHours(6)
                    && x.Trainer.EntityId != user.EntityId)
                .OrderBy(x => x.StartTime)
                .ForEach(appt =>
            {
                if (appt.StartTime != null && appt.EndTime != null)
                {
                    LocationBlockedSlots loc = blocked.Location
                        .FirstOrDefault(x => x.LocationId == appt.Location.EntityId)
                        ?? new LocationBlockedSlots();
                    if (loc.LocationId == 0)
                    {
                        loc.LocationId = appt.Location.EntityId;
                        loc.TimeSlots = new List<BlockedSlot>();
                        loc.Name = appt.Location.Name;
                        blocked.Location.Add(loc);
                    }
                    var startD = new DateTime((appt.StartTime ?? new DateTime()).Ticks);
                    while (startD < appt.EndTime)
                    {
                        var slotString = startD.ToString("M/d/yyyy h:mm tt");
                        BlockedSlot slot = loc.TimeSlots
                            .FirstOrDefault(x => x.TimeSlot == slotString)
                            ?? new BlockedSlot();
                        if (slot.TimeSlot == null)
                        {
                            slot.TimeSlot = slotString;
                            loc.TimeSlots.Add(slot);
                        }
                        slot.count++;
                        if ((loc.LocationId == 2 && slot.count >= location2CutOff)
                        || (loc.LocationId == 1 && slot.count >= location1CutOff))
                        {
                            slot.Blocked = true;
                        }

                        startD = startD.AddMinutes(15);
                    }
                }

            });
            blocked.Location.ForEach(loc => loc.TimeSlots = loc.TimeSlots.Where(x => x.Blocked).ToList());
            return blocked;
        }

        private void CreateBlockingEvents(BlockedSlotsByLocation blocked, List<CalendarEvent> events, int locationsAvailableToView)
        {
            if(blocked.Location == null)
            {
                return;
            }
            for (int i = 0; i < blocked.Location.Count(); i++)
            {
                var loc = blocked.Location[i];
                if (loc.LocationId != locationsAvailableToView)
                {
                    for (int j = 0; j < loc.TimeSlots.Count(); j++)
                    {
                        var k = j;
                        while (k < loc.TimeSlots.Count() - 1 &&
                            DateTime.Parse(loc.TimeSlots[k].TimeSlot).AddMinutes(15)
                            == DateTime.Parse(loc.TimeSlots[k + 1].TimeSlot))
                        {
                            k = k + 1;
                        }
                        var calendarEvent = new CalendarEvent
                        {
                            EntityId = 0,
                            start = DateTime.Parse(loc.TimeSlots[j].TimeSlot).ToString(),
                            end = DateTime.Parse(loc.TimeSlots[k].TimeSlot).AddMinutes(15).ToString(),
                            color = "#808080",
                            locationId = loc.LocationId,
                            title = loc.Name,
                            editable = false
                        };
                        events.Add(calendarEvent);
                        j = k;
                    }
                }
            }
        }
        private void GetValue(Appointment x, List<CalendarEvent> events, User user, bool canSeeOthers, int locationsAvailableToView)
        {
            var calendarEvent = new CalendarEvent
            {
                EntityId = x.EntityId,
                start = x.StartTime.ToString(),
                end = x.EndTime.ToString(),
                color = x.Trainer.Color,
                trainerId = x.Trainer.EntityId,
                locationId = x.Location.EntityId,
                appointmentType = x.AppointmentType,
                editable = true
            };
            if (x.Clients.Count() > 1)
            {
                calendarEvent.title = x.Location.Name + ": Multiple";
            }
            else if (x.Clients.Count() == 1)
            {
                calendarEvent.title = x.Location.Name + ": " + x.Clients.FirstOrDefault().FullNameLNF;
            }

            if (x.Trainer != user && !canSeeOthers 
                || (canSeeOthers 
                    && locationsAvailableToView != x.Location.EntityId 
                    && locationsAvailableToView != 0))
            {
                return;
                //calendarEvent.color = "#fffff";
                //calendarEvent.title = string.Empty;
                //calendarEvent.className = "hiddenEvent";
                //calendarEvent.trainerId= 0;
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


    public class BlockedSlot
    {
        public string TimeSlot { get; set; }
        public bool Blocked { get; set; }
        public int count { get; set; }
    }
    public class LocationBlockedSlots
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
        public List<BlockedSlot> TimeSlots { get; set; }
    }
    public class BlockedSlotsByLocation
    {
        public List<LocationBlockedSlots> Location { get; set; }
    }

    public class CalendarEventPayload
    {
        public List<CalendarEvent> Events { get; set; }
        public BlockedSlotsByLocation BlockedSlotsByLocation { get; set; }
    }
}