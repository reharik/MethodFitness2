using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;
using MethodFitness.Web.Grids;
using StructureMap;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class TrainerSessionVerificationListController : MFController
    {
        private readonly IEntityListGrid<TrainerSessionDto>  _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;

        public TrainerSessionVerificationListController(SessionVerificationListGrid grid,
            IDynamicExpressionQuery dynamicExpressionQuery)
        {
            _grid = grid;
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
                addUpdateUrl = UrlContext.GetUrlForAction<TrainerSessionVerificationController>(x => x.Display(null))
            };
            return new CustomJsonResult(model);
        }

        public JsonResult Items(GridItemsRequestModel input)
        {
            var user = (User) input.User;
            var items = _dynamicExpressionQuery.PerformQuery(user.TrainerSessionVerifications, input.filters);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items,user);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }
}