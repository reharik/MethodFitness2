using System.Web.Mvc;

namespace MethodFitness.Web.Areas.Portfolio
{
    public class PortfolioAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Portfolio";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Portfolio_default",
                "Portfolio/{controller}/{action}/{EntityId}",
                new { controller="AssistantSetList", action = "AssistantSetList", EntityId = UrlParameter.Optional }
            );
        }
    }
}
