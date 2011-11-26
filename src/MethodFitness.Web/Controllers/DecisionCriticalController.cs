using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Html;

namespace MethodFitness.Web.Controllers
{
    public class MethodFitnessController:MFController
    {
         public ActionResult Home(ViewModel input)
         {
             var methodFitnessViewModel = new MethodFitnessViewModel
                                                 {
                                                     UserProfileUrl = UrlContext.GetUrlForAction<UserProfileController>(x => x.UserProfile(null))
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

    }
}