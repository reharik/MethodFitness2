using System.Collections.Generic;
using CC.Core.Html.Menu;
using CC.Security;
using MF.Core.Services;
using MF.Web.Areas.Billing.Controllers;
using MF.Web.Areas.Reporting.Controllers;
using MF.Web.Areas.Schedule.Controllers;
using MF.Web.Controllers;
using MethodFitness.Web;

namespace MF.Web.Menus
{
    public class MainMenu : IMenuConfig
    {
        private readonly IMenuBuilder _builder;
        private readonly ISessionContext _sessionContext;

        public MainMenu(IMenuBuilder builder, ISessionContext sessionContext)
        {
            _builder = builder;
            _sessionContext = sessionContext;
        }

        public IList<MenuItem> Build(bool withoutPermissions = false)
        {
            return DefaultMenubuilder(withoutPermissions);
        }

        private IList<MenuItem> DefaultMenubuilder(bool withoutPermissions = false)
        {
            IUser user = null;
            if (!withoutPermissions)
            {
                user = _sessionContext.GetCurrentUser();
            }
            var builder =
                _builder.CreateTagNode<AppointmentCalendarController>(WebLocalizationKeys.CALENDAR).Route("calendar")
                        .CreateTagNode<ClientListController>(WebLocalizationKeys.CLIENTS)
                        .CreateTagNode<TrainerSessionViewController>(WebLocalizationKeys.SESSION_REPORT)
                        .CreateNode(WebLocalizationKeys.SESSION_VERIFICATION)
                        .HasChildren()
                            .CreateTagNode<TrainerSessionVerificationListController>(WebLocalizationKeys.HISTORICAL)
                            .CreateTagNode<TrainerSessionVerificationController>(WebLocalizationKeys.CURRENT)
                        .EndChildren()
                        .CreateNode(WebLocalizationKeys.ADMIN_TOOLS, "tools")
                        .HasChildren()
                            .CreateTagNode<LocationListController>(WebLocalizationKeys.LOCATIONS)
                            .CreateTagNode<BaseSessionRateController>(WebLocalizationKeys.BASE_RATES)
                            .CreateTagNode<TrainerListController>(WebLocalizationKeys.TRAINERS)
                            .CreateNode(WebLocalizationKeys.REPORTS)
                                .HasChildren()
                                    .CreateTagNode<DailyPaymentsController>(WebLocalizationKeys.DAILY_PAYMENTS)
                                    .CreateTagNode<TrainerMetricController>(WebLocalizationKeys.TRAINER_METRIC)
                                    .CreateTagNode<ActivityController>(WebLocalizationKeys.ACTIVITY)
                                .EndChildren()
                        .EndChildren();
            var list = builder.MenuTree(user);
            return list;
        }
    }
}













