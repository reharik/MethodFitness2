using System.Collections.Generic;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using MethodFitness.Core;
using MethodFitness.Web.Areas.Schedule.Controllers;

namespace MethodFitness.Web.Models
{
    public class CalendarViewModel : ViewModel
    {
        public CalendarDefinition CalendarDefinition { get; set; }
        public string DeleteUrl { get; set; }
        public Location Location { get; set; }
        public IEnumerable<SelectListItem> _LocationList { get; set; }
        public string CalendarUrl { get; set; }
        public string Loc { get; set; }

        public List<TrainerLegendDto> Trainers { get; set; }
    }

    public class GetEventsViewModel : ViewModel
    {
        public string TrainerIds { get; set; }
        public double start { get; set; }
        public double end { get; set; }

        public int Loc { get; set; }
    }

    public class CalendarDefinition
    {
        public string Url { get; set; }
        public string EventsUrlBase { get; set; }
        public string Title { get; set; }

        public string AddUpdateUrl { get; set; }

        public string EventChangedUrl { get; set; }

        public string DisplayUrl { get; set; }

        public bool CanEditPastAppointments { get; set; }

        public bool CanEnterRetroactiveAppointments { get; set; }
       
        public string DeleteUrl { get; set; }

        public bool CanEditOthersAppointments { get; set; }

        public bool CanSeeOthersAppointments { get; set; }

        public int TrainerId { get; set; }

        public string AddUpdateRoute { get; set; }

        public string DisplayRoute { get; set; }
    }

    public class CalendarEvent
    {
        public long EntityId { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string url { get; set; }
        public string className { get; set; }
        public string color { get; set; }

        public int trainerId { get; set; }
    }
}