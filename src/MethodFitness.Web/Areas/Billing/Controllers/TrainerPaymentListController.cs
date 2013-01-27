using System.Web.Mvc;
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
using NHibernate.Linq;
using System.Linq;

namespace MethodFitness.Web.Areas.Billing.Controllers
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

            var trainer = _repository.Find<User>(input.EntityId);
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

            var trainer = _repository.Query<User>(x=>x.EntityId==input.ParentId).FetchMany(x=>x.TrainerPayments).FirstOrDefault();
            var items = _dynamicExpressionQuery.PerformQuery(trainer.TrainerPayments,input.filters);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items,user);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }
}