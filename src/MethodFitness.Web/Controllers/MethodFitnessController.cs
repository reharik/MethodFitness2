using System.Collections.Generic;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Html;
using MethodFitness.Web.Services.ViewOptions;

namespace MethodFitness.Web.Controllers
{
    public class MethodFitnessController:MFController
    {
        private readonly IViewOptionConfig _viewOptionConfig;

        public MethodFitnessController(IViewOptionConfig viewOptionConfig)
        {
            _viewOptionConfig = viewOptionConfig;
        }

        public ActionResult Home(ViewModel input)
         {
             var methodFitnessViewModel = new MethodFitnessViewModel
                                                 {
                                                     SerializedRoutes = _viewOptionConfig.Build(true)
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

        public IList<ViewOption> SerializedRoutes { get; set; }
    }
}