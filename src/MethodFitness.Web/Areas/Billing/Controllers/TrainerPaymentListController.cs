using System;
using System.Linq;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Html.Grid;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class TrainerPaymentListController : MFController
    {
        private readonly IEntityListGrid<TrainerPayment> _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;

        public TrainerPaymentListController(IEntityListGrid<TrainerPayment> grid,
            IDynamicExpressionQuery dynamicExpressionQuery,
            IRepository repository,
            ISaveEntityService saveEntityService )
        {
            _grid = grid;
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _repository = repository;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var trainer = _repository.Find<User>(input.EntityId);
            var url = UrlContext.GetUrlForAction<TrainerPaymentListController>(x => x.TrainerPayments(null),AreaName.Billing) + "?ParentId="+input.EntityId;
            var model = new TrainersPaymentListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url),
                _Title = trainer.FullNameFNF+"'s " + WebLocalizationKeys.PAYMENTS,
                TrainersName = trainer.FullNameFNF,
                EntityId = trainer.EntityId
            };
            model.headerButtons.Add("return");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TrainerPayments(GridItemsRequestModel input)
        {
            var trainer = _repository.Find<Trainer>(input.ParentId);
            var items = _dynamicExpressionQuery.PerformQueryWithItems(trainer.TrainerPayments,input.filters);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items);
            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}