using System;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using CC.Security;
using MF.Core.CoreViewModelAndDTOs;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Config;
using MF.Web.Controllers;
using MF.Web;
using StructureMap;

namespace MF.Web.Areas.Billing.Controllers
{
    public class TrainerSessionViewController : MFController
    {

        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;
        private IEntityListGrid<TrainerSessionDto> _grid;

        public TrainerSessionViewController(
            IDynamicExpressionQuery dynamicExpressionQuery,
            IRepository repository)
        {
            _grid = ObjectFactory.Container.GetInstance<IEntityListGrid<TrainerSessionDto>>("SessionVerification");
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _repository = repository;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<TrainerSessionViewController>(x => x.TrainerSessions(null), AreaName.Billing) + "?ParentId=" + input.EntityId;
            var user = (User)input.User;
            var model = new TrainersPaymentListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url, user),
                _Title = user.FullNameFNF + "'s " + WebLocalizationKeys.PAYMENT_AMOUNT,
                TrainersName = user.FullNameFNF,
                EntityId = user.EntityId,
            };
            return new CustomJsonResult(model);
        }

        public JsonResult TrainerSessions(TrainerPaymentGridItemsRequestModel input)
        {
            var endDate = input.endDate.HasValue ? input.endDate : DateTime.Now;

            var items = _dynamicExpressionQuery.PerformQuery<TrainerSessionDto>(input.filters, x => x.TrainerId == input.User.EntityId && x.AppointmentDate <= endDate && !x.TrainerVerified);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items, (IUser)input.User);
            return new CustomJsonResult(gridItemsViewModel);
        }
    }

   
}
