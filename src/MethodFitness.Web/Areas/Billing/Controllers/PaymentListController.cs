using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class PaymentListController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<Payment> _paymentListGrid;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;

        public PaymentListController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<Payment> paymentListGrid,
            IRepository repository,ISessionContext sessionContext)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _paymentListGrid = paymentListGrid;
            _repository = repository;
            _sessionContext = sessionContext;
        }

        public CustomJsonResult ItemList(ViewModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            var url = UrlContext.GetUrlForAction<PaymentListController>(x => x.Payments(null),AreaName.Billing) + "?ParentId=" + input.EntityId;
            var model = new ListViewModel()
            {
                addUpdateUrl = UrlContext.GetUrlForAction<PaymentController>(x => x.AddUpdate(null), AreaName.Billing) + "?ParentId=" + input.EntityId,
                gridDef = _paymentListGrid.GetGridDefinition(url,user),
                _Title = WebLocalizationKeys.PAYMENTS.ToString(),
                ParentId = input.EntityId
            };
            model.headerButtons.Add("new");
            return new CustomJsonResult(model);
        }

        public CustomJsonResult Payments(GridItemsRequestModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            var client = _repository.Find<Client>(input.ParentId);
            var items = _dynamicExpressionQuery.PerformQuery(client.Payments, input.filters);
            var gridItemsViewModel = _paymentListGrid.GetGridItemsViewModel(input.PageSortFilter, items,user);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }
}