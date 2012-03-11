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
    public class TrainerPaymentController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<Payment> _TrainerPaymentGrid;
        private readonly IRepository _repository;

        public TrainerPaymentController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<Payment> TrainerPaymentGrid,
            IRepository repository)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _TrainerPaymentGrid = TrainerPaymentGrid;
            _repository = repository;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<TrainerPaymentController>(x => x.Payments(null),AreaName.Billing) + "?ParentId=" + input.ParentId;
            var model = new ListViewModel()
            {
                addUpdateUrl = UrlContext.GetUrlForAction<PaymentController>(x => x.AddUpdate(null), AreaName.Billing) + "?ParentId=" + input.ParentId,
                gridDef = _TrainerPaymentGrid.GetGridDefinition(url),
                Title = WebLocalizationKeys.PAYMENTS.ToString() 
            };
            return View(model);
        }

        public JsonResult Payments(GridItemsRequestModel input)
        {
            var client = _repository.Find<Client>(input.ParentId);
            var items = _dynamicExpressionQuery.PerformQueryWithItems(client.Payments, input.filters);
            var gridItemsViewModel = _TrainerPaymentGrid.GetGridItemsViewModel(input.PageSortFilter, items);
            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}