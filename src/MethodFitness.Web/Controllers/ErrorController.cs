using System.Web.Mvc;
using MethodFitness.Core;

namespace MethodFitness.Web.Controllers
{
    public class ErrorController:Controller  
    {
        public ActionResult Trouble()
        {
            return View("Error");
        }
    }
}