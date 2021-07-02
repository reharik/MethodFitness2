
namespace MF.Web.Areas.Billing
{
    public class BillingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Billing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Billing_default",
                "Billing/{controller}/{action}/{EntityId}",
                new { action = "Index", EntityId = UrlParameter.Optional }
            );
        }
    }
}
