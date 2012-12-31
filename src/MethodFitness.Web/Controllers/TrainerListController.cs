using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.Html;
using CC.Core.Services;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class TrainerListController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<User> _trainerListGrid;
        private readonly ISessionContext _sessionContext;

        public TrainerListController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<User> trainerListGrid, ISessionContext sessionContext)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _trainerListGrid = trainerListGrid;
            _sessionContext = sessionContext;
        }

        public JsonResult ItemList(ViewModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            var url = UrlContext.GetUrlForAction<TrainerListController>(x => x.Trainers(null));
            var model = new ListViewModel()
            {
                addUpdateUrl = UrlContext.GetUrlForAction<TrainerController>(x => x.AddUpdate(null)),
                gridDef = _trainerListGrid.GetGridDefinition(url,user),
                searchField = "LastName"
            };
            model.headerButtons.Add("new");
            return new CustomJsonResult(model);
        }

        public JsonResult Trainers(GridItemsRequestModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            //TODO find way to deal with string here
            var items = _dynamicExpressionQuery.PerformQuery<User>(input.filters, x=>x.UserRoles.Any(r=>r.Name == "Trainer" ));
            var gridItemsViewModel = _trainerListGrid.GetGridItemsViewModel(input.PageSortFilter, items,user);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }
}