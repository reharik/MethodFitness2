using System.Web.Mvc;

namespace MF.Web.Controllers
{
    public class ErrorController:Controller  
    {
        public ActionResult Trouble()
        {
            return View("Error");
        }
    }
}