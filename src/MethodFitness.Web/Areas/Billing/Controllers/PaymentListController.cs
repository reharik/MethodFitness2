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
        private readonly ISessionContext _sessionContext;
        private readonly IRepository _repository;

        public PaymentListController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<Payment> paymentListGrid,
            ISessionContext sessionContext,
            IRepository repository)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _paymentListGrid = paymentListGrid;
            _sessionContext = sessionContext;
            _repository = repository;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<PaymentListController>(x => x.Payments(null));
            var model = new ListViewModel()
            {
                AddUpdateUrl = UrlContext.GetUrlForAction<PaymentController>(x => x.AddUpdate(null)),
                GridDefinition = _paymentListGrid.GetGridDefinition(url),
                Title = WebLocalizationKeys.CLIENTS.ToString() 
            };
            return View(model);
        }

        public JsonResult Payments(GridItemsRequestModel input)
        {
            var userEntityId = _sessionContext.GetUserEntityId();
            var user = _repository.Find<User>(userEntityId);
            IQueryable<Payment> items;
            if (user.UserRoles.Any(x => x.Name == "Administrator"))
            {
                items = _dynamicExpressionQuery.PerformQuery<Payment>(input.filters);
            }else
            {
                items = _dynamicExpressionQuery.PerformQueryWithItems(user.Payments, input.filters);
            }
            var gridItemsViewModel = _paymentListGrid.GetGridItemsViewModel(input.PageSortFilter, items);
            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}