using System.Linq;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.Html;
using CC.Core.Core.Html.Menu;
using CC.Core.Core.Services;
using MF.Core.Domain;
using MF.Core.Services;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Config;
using Microsoft.AspNetCore.Mvc;

namespace MF.Web.Controllers
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
            var model = new TrainerListViewModel()
            {
                ArchiveTrainerUrl = UrlContext.GetUrlForAction<TrainerController>(x => x.TrainerStatus(null)),
                addUpdateUrl = UrlContext.GetUrlForAction<TrainerController>(x => x.AddUpdate(null)),
                gridDef = _trainerListGrid.GetGridDefinition(url,user)
            };
            model.headerButtons.Add("new");
            model.headerButtons.Add("delete");
            model.headerButtons.Add("toggleArchived");
            return new JsonResult(model);
        }

        public JsonResult Trainers(GridItemsRequestModel input)
        {
            if(input.filters == null) {
                input.filters = "{'group': 'AND', rules: [{'op':'toggle', 'field': 'Archived', 'data': 'hide' }]}";
            }
            var user = _sessionContext.GetCurrentUser();
            //TODO find way to deal with string here
            var items = _dynamicExpressionQuery.PerformQuery<User>(input.filters, x=>x.UserRoles.Any(r=>r.Name == "Trainer" ));
            var gridItemsViewModel = _trainerListGrid.GetGridItemsViewModel(input.PageSortFilter, items,user);
            return new JsonResult(gridItemsViewModel);
        }
    }

    public class TrainerListViewModel : ListViewModel
    {
        public string ArchiveTrainerUrl { get; set; }
    }
}