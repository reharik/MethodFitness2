using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.DomainTools;
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
using System.Linq;

namespace MF.Web.Areas.Billing.Controllers
{
    public class ClientPurchaseListController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<Payment> _paymentListGrid;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;

        public ClientPurchaseListController(IDynamicExpressionQuery dynamicExpressionQuery,
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
            var client = _repository.Find<Client>(input.EntityId);
            var url = UrlContext.GetUrlForAction<ClientPurchaseListController>(x => x.Purchase(null),AreaName.Billing) + "?ParentId=" + input.EntityId;
            var model = new ListViewModel()
            {
                addUpdateUrl = UrlContext.GetUrlForAction<ClientPurchaseController>(x => x.AddUpdate(null), AreaName.Billing) + "?ParentId=" + input.EntityId,
                gridDef = _paymentListGrid.GetGridDefinition(url,user),
                _Title = WebLocalizationKeys.CLIENT_PAYMENTS.ToFormat(client.FullNameFNF),
                ParentId = input.EntityId
            };
            model.headerButtons.Add("new");
            model.headerButtons.Add("delete");
            return new CustomJsonResult(model);
        }

        public CustomJsonResult Purchase(GridItemsRequestModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            var client = _repository.Find<Client>(input.ParentId);
            var items = _dynamicExpressionQuery.PerformQuery(client.Payments.OrderBy(x=>x.CreatedDate), input.filters);
            var gridItemsViewModel = _paymentListGrid.GetGridItemsViewModel(input.PageSortFilter, items,user);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }
}