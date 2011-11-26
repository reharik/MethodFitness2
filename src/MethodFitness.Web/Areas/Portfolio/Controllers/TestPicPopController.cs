using System.Web.Mvc;
using DecisionCritical.Core.Domain;
using DecisionCritical.Web.Controllers;
using DecisionCritical.Web.Models;

namespace DecisionCritical.Web.Areas.Portfolio.Controllers
{
    public class TestPicPopController : DCIController
    {
        public ActionResult TestPicPop(AssetViewModel<Committee> input)
        {
            var model = new CommitteeListViewModel { };
            return View("TestPicPop", model);
        }
    }

}