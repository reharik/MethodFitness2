using System;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using CC.Security;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.NamedQueries;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;
using MethodFitness.Web.Grids;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using StructureMap;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class VerifiedTrainerSessionsController : MFController
    {

        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;
        private IEntityListGrid<TrainerSessionDto> _grid;

        public VerifiedTrainerSessionsController(
            IDynamicExpressionQuery dynamicExpressionQuery,
            IRepository repository)
        {
            _grid = ObjectFactory.Container.GetInstance<IEntityListGrid<TrainerSessionDto>>("SessionVerification");
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _repository = repository;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var trainerSessionVerification = _repository.Find<TrainerSessionVerification>(input.EntityId);
            var url = UrlContext.GetUrlForAction<VerifiedTrainerSessionsController>(x => x.TrainerSessions(null), AreaName.Billing) + "?ParentId=" + input.EntityId;
            var user = (User)input.User;
            var model = new TrainersPaymentListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url, user),
                EntityId = user.EntityId,
                _Title = "Created on: " + trainerSessionVerification.CreatedDate.Value.ToShortDateString()+".  Total Verfied: $"+trainerSessionVerification.Total
            };
            return new CustomJsonResult(model);
        }

        public JsonResult TrainerSessions(TrainerPaymentGridItemsRequestModel input)
        {
            var items = _dynamicExpressionQuery.PerformQuery<TrainerSessionDto>(input.filters, x => x.TrainerSessionVerificationId == input.ParentId);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items, (IUser)input.User);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }

   
}
