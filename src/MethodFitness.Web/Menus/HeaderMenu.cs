using System.Collections.Generic;
using CC.Core.Html.Menu;
using CC.Security;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Config
{
    public class HeaderMenu : IMenuConfig
    {
        private readonly IMenuBuilder _builder;
        private readonly ISessionContext _sessionContext;

        public HeaderMenu(IMenuBuilder builder, ISessionContext sessionContext)
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
            if(!withoutPermissions)
            {
                user = _sessionContext.GetCurrentUser();
            }
            return _builder
                .CreateNode<MethodFitnessController>(c => c.Home(null), WebLocalizationKeys.DASHBOARD)
                //.CreateNode<PortfolioDashboardController>(c => c.PortfolioDashboard(null), WebLocalizationKeys.ASSESTS,AreaName.Portfolio)
//                .CreateNode(WebLocalizationKeys.CALNEDAR)
//                .CreateNode(WebLocalizationKeys.LEARNING)
//               // .CreateNode<PortfolioDashboardController>("/MethodFitness/Home#", x=>x.Display(null), WebLocalizationKeys.PORTFOLIOS, AreaName.Portfolio, "selected") 
//                .CreateNode(WebLocalizationKeys.EVALUATIONS)
                .MenuTree(user);
        }
    }
}