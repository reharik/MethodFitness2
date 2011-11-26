using System.Web.Mvc;
using DecisionCritical.Core.Domain;
using DecisionCritical.Core.Html;
using DecisionCritical.Core.Services;
using DecisionCritical.Web.Controllers;
using DecisionCritical.Web.Grids;
using DecisionCritical.Web.Models;

namespace DecisionCritical.Web.Areas.Portfolio.Controllers
{
    public class AssetListController : DCIController
    {
        private IAssetListGrid _assetListGrid;
        private ISessionContext _getIds;
        private DynamicExpressionQuery _dynamicExpression;

        public AssetListController(IAssetListGrid assetListGrid, ISessionContext getIds, DynamicExpressionQuery dynamicExpression)
        {
            _dynamicExpression = dynamicExpression;
            _getIds = getIds;
            _assetListGrid = assetListGrid;
        }
        
        public ActionResult AssetList(AssetListViewModel input)
        {
            var title = WebLocalizationKeys.ASSETS_TITLE;
            var urlForAction = UrlContext.GetUrlForAction<AssetListController>(z => z.Assets(null));
            var definition = _assetListGrid.GetGridDefinition(urlForAction);
            var model = new AssetListViewModel
                                          {
                                              //TODO:put Add on model
                                              GridDefinition = definition,
                                              CrudTitle = title.ToString()
                                          };
            return View(model);
        }

        public JsonResult Assets(GridItemsRequestModel input)
        {
            var userEntityId = _getIds.GetUserEntityId();
            var items = _dynamicExpression.PerformQuery<Asset>(input.filters,z=>z.User.EntityId==userEntityId);
            var gridItemsViewModel = _assetListGrid.GetGridItemsViewModel(input.PageSortFilter,items);

            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}