using System.Collections.Generic;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using MethodFitness.Web.Services.ViewOptions;

namespace MethodFitness.Web.Controllers
{
    public class MethodFitnessController:MFController
    {
        private readonly IRouteTokenConfig _routeTokenConfig;

        public MethodFitnessController(IRouteTokenConfig routeTokenConfig)
        {
            _routeTokenConfig = routeTokenConfig;
        }

        public ActionResult Home(ViewModel input)
         {
             var methodFitnessViewModel = new MethodFitnessViewModel
                                                 {
                                                     SerializedRoutes = _routeTokenConfig.Build(true)
                                                 };
             return View(methodFitnessViewModel);
         }
    }

    public class MethodFitnessViewModel : ViewModel
    {
        public string c { get; set; }
        public string u { get; set; }
        public string mode { get; set; }
        public string FirstTimeUrl { get; set; }
        public string UserProfileUrl { get; set; }

        public IList<RouteToken> SerializedRoutes { get; set; }
    }
}