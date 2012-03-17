using System.Collections.Generic;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html.Menu;
using KnowYourTurf.Web.Config;
using MethodFitness.Web.Areas.Reports.Controllers;
using MethodFitness.Web.Areas.Schedule.Controllers;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Menus
{
    public class MainMenu : IMenuConfig
    {
        private readonly IMenuBuilder _builder;

        public MainMenu(IMenuBuilder builder)
        {
            _builder = builder;
        }

        public IList<MenuItem> Build(bool withoutPermissions = false)
        {
            return DefaultMenubuilder(withoutPermissions);
        }

        private IList<MenuItem> DefaultMenubuilder(bool withoutPermissions = false)
        {
            return _builder
                .CreateTagNode<AppointmentCalendarController>(WebLocalizationKeys.CALENDAR).Route("calendar")
                .CreateTagNode<ClientListController>(WebLocalizationKeys.CLIENTS)
                .CreateNode(WebLocalizationKeys.ADMIN_TOOLS, "tools")
                    .HasChildren()
                        .CreateTagNode<TrainerListController>(WebLocalizationKeys.TRAINERS)
                        .CreateTagNode<TimeSheetController>(WebLocalizationKeys.TIME_SHEET)
                    .EndChildren()
            .MenuTree(withoutPermissions);
        }
    }
}













