using System.Collections.Generic;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html.Menu;
using MethodFitness.Web;
using MethodFitness.Web.Areas.Portfolio.Controllers;
using MethodFitness.Web.Controllers;

namespace KnowYourTurf.Web.Config
{
    public class HeaderMenu : IMenuConfig
    {
        private readonly IMenuBuilder _builder;

        public HeaderMenu(IMenuBuilder builder)
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
                .CreateNode<MethodFitnessController>(c => c.Home(null), WebLocalizationKeys.DASHBOARD)
                //.CreateNode<PortfolioDashboardController>(c => c.PortfolioDashboard(null), WebLocalizationKeys.ASSESTS,AreaName.Portfolio)
                .CreateNode(WebLocalizationKeys.CALNEDAR)
                .CreateNode(WebLocalizationKeys.LEARNING)
               // .CreateNode<PortfolioDashboardController>("/MethodFitness/Home#", x=>x.Display(null), WebLocalizationKeys.PORTFOLIOS, AreaName.Portfolio, "selected") 
                .CreateNode(WebLocalizationKeys.EVALUATIONS)
                .MenuTree(withoutPermissions);
        }
    }
}