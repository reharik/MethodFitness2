using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using MF.Core.Domain;
using MF.Core.Services;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Config;
using MF.Web;

namespace MF.Web.Controllers
{
    public class ClientListController:MFController
    {
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IEntityListGrid<Client> _clientListGrid;
        private readonly ISessionContext _sessionContext;
        private readonly IRepository _repository;

        public ClientListController(IDynamicExpressionQuery dynamicExpressionQuery,
            IEntityListGrid<Client> clientListGrid,
            ISessionContext sessionContext,
            IRepository repository)
        {
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _clientListGrid = clientListGrid;
            _sessionContext = sessionContext;
            _repository = repository;
        }

        public JsonResult ItemList(ViewModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            var url = UrlContext.GetUrlForAction<ClientListController>(x => x.Clients(null));
            var model = new ListViewModel()
            {
                addUpdateUrl = UrlContext.GetUrlForAction<ClientController>(x => x.AddUpdate(null)),
                deleteMultipleUrl = UrlContext.GetUrlForAction<ClientController>(x => x.DeleteMultiple(null)),
               // PaymentUrl = UrlContext.GetUrlForAction<PaymentListController>(x => x.ItemList(null),AreaName.Billing),
                gridDef = _clientListGrid.GetGridDefinition(url,user),
                _Title = WebLocalizationKeys.CLIENTS.ToString()
            };
            model.headerButtons.Add("new");
            model.headerButtons.Add("delete");
            return new CustomJsonResult(model);
        }

        public JsonResult Clients(GridItemsRequestModel input)
        {
            var userEntityId = _sessionContext.GetUserId();
            var trainer = _repository.Find<User>(userEntityId);
            IQueryable<Client> items;
            if (trainer.UserRoles.Any(x => x.Name == "Administrator"))
            {
                items = _dynamicExpressionQuery.PerformQuery<Client>(input.filters);
            }else
            {
                items = _dynamicExpressionQuery.PerformQuery(trainer.Clients, input.filters);
            }
            var gridItemsViewModel = _clientListGrid.GetGridItemsViewModel(input.PageSortFilter, items, trainer);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }
    
}