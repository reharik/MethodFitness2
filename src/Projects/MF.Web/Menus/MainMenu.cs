using System.Collections.Generic;
using CC.Core.Core.Html.Menu;
using CC.Core.Security;
using MF.Core.Services;
using MF.Web.Areas.Billing.Controllers;
using MF.Web.Areas.Reporting.Controllers;
using MF.Web.Areas.Schedule.Controllers;
using MF.Web.Controllers;
using CC.Core.Security.Interfaces;
using CC.Core.Security.Model;
using MF.Core;
namespace MF.Web.Menus
{
    public class MainMenu : IMenuConfig
    {
        private readonly IMenuBuilder _builder;
        private readonly ISessionContext _sessionContext;
		private readonly IAuthorizationRepository authorizationRepository;
        private readonly ILogger _logger;

        public MainMenu(IMenuBuilder builder, ISessionContext sessionContext, IAuthorizationRepository authorizationRepository, ILogger logger)
        {
            _builder = builder;
            _sessionContext = sessionContext;
            _logger = logger;
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
                UsersGroup[] usersGroups = authorizationRepository.GetAssociatedUsersGroupFor(user);
                foreach (var x in usersGroups)
                {
                    _logger.LogInfo(x.Name);
                }
            }
            var builder =
                _builder.CreateTagNode<AppointmentCalendarController>(WebLocalizationKeys.CALENDAR).Route("calendar")
                        .CreateTagNode<ClientListController>(WebLocalizationKeys.CLIENTS)
                        .CreateTagNode<TrainerSessionViewController>(WebLocalizationKeys.SESSION_REPORT)
                        .CreateTagNode<TrainerPaymentListController>(WebLocalizationKeys.PAYMENT_HISTORY)
                        .CreateTagNode<TrainerSessionVerificationController>(WebLocalizationKeys.SESSION_VERIFICATION)
                        .CreateNode(WebLocalizationKeys.ADMIN_TOOLS, "tools")
                        .HasChildren()
                            .CreateTagNode<LocationListController>(WebLocalizationKeys.LOCATIONS)
                            .CreateTagNode<BaseSessionRateController>(WebLocalizationKeys.BASE_RATES)
                            .CreateTagNode<TrainerListController>(WebLocalizationKeys.TRAINERS)
                            .CreateNode(WebLocalizationKeys.REPORTS)
                                .HasChildren()
                                   // .CreateTagNode<ManagerController>(WebLocalizationKeys.MANAGER_REPORT)
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













