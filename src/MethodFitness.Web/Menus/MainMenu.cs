using System.Collections.Generic;
using CC.Core.Html.Menu;
using CC.Security;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Config;
using MethodFitness.Web.Areas.Schedule.Controllers;

namespace MethodFitness.Web.Menus
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
                        .CreateTagNode<TrainerSessionVerificationController>(WebLocalizationKeys.SESSION_REPORT)
                        .CreateNode(WebLocalizationKeys.ADMIN_TOOLS, "tools")
                        .HasChildren()
                            .CreateTagNode<TrainerListController>(WebLocalizationKeys.TRAINERS)
                        .EndChildren();
            var list = builder.MenuTree(user);
            return list;
        }
    }
}













