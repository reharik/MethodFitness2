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
    public class PaymentListController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<Payment> _paymentListGrid;

        public PaymentListController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<Payment> paymentListGrid)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _paymentListGrid = paymentListGrid;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<PaymentListController>(x => x.Payments(null));
            var model = new ListViewModel()
            {
                AddUpdateUrl = UrlContext.GetUrlForAction<PaymentController>(x => x.AddUpdate(null)),
                GridDefinition = _paymentListGrid.GetGridDefinition(url),
                Title = WebLocalizationKeys.PAYMENTS.ToString() 
            };
            return View(model);
        }

        public JsonResult Payments(GridItemsRequestModel input)
        {
            var items = _dynamicExpressionQuery.PerformQuery<Payment>(input.filters);
            var gridItemsViewModel = _paymentListGrid.GetGridItemsViewModel(input.PageSortFilter, items);
            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}