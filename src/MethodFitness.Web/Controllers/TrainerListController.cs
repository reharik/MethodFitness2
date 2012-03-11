using System.Linq;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class TrainerListController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<User> _trainerListGrid;

        public TrainerListController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<User> trainerListGrid)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _trainerListGrid = trainerListGrid;
        }

        public JsonResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<TrainerListController>(x => x.Trainers(null));
            var model = new ListViewModel()
            {
                addUpdateUrl = UrlContext.GetUrlForAction<TrainerController>(x => x.AddUpdate(null)),
                gridDef = _trainerListGrid.GetGridDefinition(url)
            };
            model.headerButtons.Add("new");
            return Json(model,JsonRequestBehavior.AllowGet);
        }

        public JsonResult Trainers(GridItemsRequestModel input)
        {
            //TODO find way to deal with string here
            var items = _dynamicExpressionQuery.PerformQuery<User>(input.filters, x=>x.UserRoles.Any(r=>r.Name == "Trainer" ));
            var gridItemsViewModel = _trainerListGrid.GetGridItemsViewModel(input.PageSortFilter, items);
            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}