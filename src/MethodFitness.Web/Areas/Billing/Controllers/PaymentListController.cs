using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class PaymentListController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<Payment> _paymentListGrid;
        private readonly IRepository _repository;

        public PaymentListController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<Payment> paymentListGrid,
            IRepository repository)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _paymentListGrid = paymentListGrid;
            _repository = repository;
        }

        public JsonResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<PaymentListController>(x => x.Payments(null),AreaName.Billing) + "?ParentId=" + input.EntityId;
            var model = new ListViewModel()
            {
                addUpdateUrl = UrlContext.GetUrlForAction<PaymentController>(x => x.AddUpdate(null), AreaName.Billing) + "?ParentId=" + input.EntityId,
                gridDef = _paymentListGrid.GetGridDefinition(url),
                Title = WebLocalizationKeys.PAYMENTS.ToString(),
                ParentId = input.EntityId
            };
            model.headerButtons.Add("new");
            return Json(model,JsonRequestBehavior.AllowGet);
        }

        public JsonResult Payments(GridItemsRequestModel input)
        {
            var client = _repository.Find<Client>(input.ParentId);
            var items = _dynamicExpressionQuery.PerformQueryWithItems(client.Payments, input.filters);
            var gridItemsViewModel = _paymentListGrid.GetGridItemsViewModel(input.PageSortFilter, items);
            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}