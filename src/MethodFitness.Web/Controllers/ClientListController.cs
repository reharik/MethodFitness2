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
    public class ClientListController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<Client> _clientListGrid;

        public ClientListController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<Client> clientListGrid)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _clientListGrid = clientListGrid;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<ClientListController>(x => x.Clients(null));
            var model = new ListViewModel()
            {
                AddUpdateUrl = UrlContext.GetUrlForAction<ClientController>(x => x.AddUpdate(null)),
                GridDefinition = _clientListGrid.GetGridDefinition(url)
            };
            return View(model);
        }

        public JsonResult Clients(GridItemsRequestModel input)
        {
            //TODO find way to deal with string here
            var items = _dynamicExpressionQuery.PerformQuery<Client>(input.filters);
            var gridItemsViewModel = _clientListGrid.GetGridItemsViewModel(input.PageSortFilter, items);
            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}