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
using StructureMap;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class TrainerSessionViewController : MFController
    {
        private readonly SessionVerificationListGrid _grid;

        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;

        public TrainerSessionViewController(SessionVerificationListGrid grid,
            IDynamicExpressionQuery dynamicExpressionQuery,
            IRepository repository)
        {
            _grid = grid;
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
            var trainerSessionDtos = _repository.Query<TrainerSessionDto>(x => x.TrainerId == input.User.EntityId && x.AppointmentDate <= endDate);

            var items = _dynamicExpressionQuery.PerformQuery(trainerSessionDtos,input.filters);
//            var sessionPaymentDtos = items.Select(x => new SessionViewDto
//                                                           {
//                                                               AppointmentDate = x.Appointment.Date,
//                                                               EntityId = x.EntityId,
//                                                               FullName = x.Client.FullNameLNF,
//                                                               PricePerSession = x.Cost,
//                                                               Type = x.AppointmentType,
//                                                               InArrears = x.InArrears,
//                                                               TrainerPercentage =
//                                                                   x.Trainer.TrainerClientRates.FirstOrDefault(
//                                                                       y => y.Client == x.Client)!=null?x.Trainer.TrainerClientRates.FirstOrDefault(
//                                                                       y => y.Client == x.Client).Percent:x.Trainer.ClientRateDefault,
//                                                               TrainerPay =
//                                                                   x.Trainer.TrainerClientRates.FirstOrDefault(
//                                                                       y => y.Client == x.Client)!=null?x.Trainer.TrainerClientRates.FirstOrDefault(
//                                                                       y => y.Client == x.Client).Percent * .01 * x.Cost : x.Trainer.ClientRateDefault * .01 * x.Cost
//                                                           });
//
//
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items, (IUser)input.User);
            return new CustomJsonResult(gridItemsViewModel);
//            return null;
        }
    }

   
}
