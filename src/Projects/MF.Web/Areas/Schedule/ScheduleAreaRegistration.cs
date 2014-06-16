using System.Web.Mvc;

namespace MF.Web.Areas.Schedule
{
    public class ScheduleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Schedule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Schedule_default",
                "Schedule/{controller}/{action}/{EntityId}",
                new { action = "Index", EntityId = UrlParameter.Optional }
            );
        }
    }
}
