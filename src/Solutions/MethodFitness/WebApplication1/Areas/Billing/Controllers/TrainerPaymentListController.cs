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
using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using System.Linq;

namespace MF.Web.Areas.Billing.Controllers
{
    public class TrainerPaymentListController : MFController
    {
        private readonly IEntityListGrid<TrainerPayment> _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;

        public TrainerPaymentListController(IEntityListGrid<TrainerPayment> grid,
            IDynamicExpressionQuery dynamicExpressionQuery,
            IRepository repository,
            ISessionContext sessionContext)
        {
            _grid = grid;
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _repository = repository;
            _sessionContext = sessionContext;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            var userId = input.EntityId > 0 ? input.EntityId : user.EntityId;
            var trainer = _repository.Find<User>(userId);
            var url = UrlContext.GetUrlForAction<TrainerPaymentListController>(x => x.TrainerPayments(null),AreaName.Billing) + "?ParentId="+input.EntityId;
            var model = new TrainersPaymentListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url,user),
                _Title = trainer.FullNameFNF+"'s " + WebLocalizationKeys.PAYMENTS,
                TrainersName = trainer.FullNameFNF,
                EntityId = trainer.EntityId
            };
            model.headerButtons.Add("return");
            return new CustomJsonResult(model);
        }

        public JsonResult TrainerPayments(GridItemsRequestModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            var userId = input.ParentId > 0 ? input.ParentId : user.EntityId;
            var trainer = _repository.Query<User>(x=>x.EntityId==userId).FetchMany(x=>x.TrainerPayments).ToList().FirstOrDefault();
            var items = _dynamicExpressionQuery.PerformQuery(trainer.TrainerPayments,input.filters);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items,user);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }
}