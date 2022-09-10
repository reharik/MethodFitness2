using System.Web.Mvc;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.Enumerations;
using CC.Core.Core.Html;
using CC.Core.Core.Html.Menu;
using CC.Core.Core.Services;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Config;
using MF.Web.Controllers;
using StructureMap;
using System.Linq;

namespace MF.Web.Areas.Billing.Controllers
{
    public class TrainerSessionVerificationListController : MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private IEntityListGrid<TrainerSessionVerification> _grid;

        public TrainerSessionVerificationListController(
            IDynamicExpressionQuery dynamicExpressionQuery)
        {
            _grid = ObjectFactory.Container.GetInstance<IEntityListGrid<TrainerSessionVerification>>();
            _dynamicExpressionQuery = dynamicExpressionQuery;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var user = (User)input.User;
            var url = UrlContext.GetUrlForAction<TrainerSessionVerificationListController>(x => x.Items(null), AreaName.Billing);
            var model = new TrainersPaymentListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url,user),
                _Title = WebLocalizationKeys.PAYMENTS.ToString(),
                displayUrl = UrlContext.GetUrlForAction<VerifiedTrainerSessionsController>(x => x.ItemList(null), AreaName.Billing) + "?ParentId=" + input.EntityId,
            };
            return new CustomJsonResult(model);
        }

        public JsonResult Items(GridItemsRequestModel input)
        {
            var user = (User) input.User;
            var items = _dynamicExpressionQuery.PerformQuery(user.TrainerSessionVerifications.OrderBy(x=>x.CreatedDate), input.filters);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items,user);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }
}