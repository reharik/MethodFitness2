using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Controllers
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
                _Title = WebLocalizationKeys.CLIENTS.ToString(),
                searchField = "LastName"
            };
            model.headerButtons.Add("new");
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