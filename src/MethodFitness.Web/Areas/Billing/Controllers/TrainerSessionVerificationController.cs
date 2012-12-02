using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Html.Grid;
using CC.Core.Services;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;
using NHibernate.Linq;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class TrainerSessionVerificationController : MFController
    {
        private readonly IEntityListGrid<SessionVerificationDto> _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;

        public TrainerSessionVerificationController(IEntityListGrid<SessionVerificationDto> grid,
            IDynamicExpressionQuery dynamicExpressionQuery,
            IRepository repository, ISessionContext sessionContext)
        {
            _grid = grid;
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _repository = repository;
            _sessionContext = sessionContext;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<TrainerSessionVerificationController>(x => x.TrainerSessions(null), AreaName.Billing) + "?ParentId=" + input.EntityId;
            var user = (User)input.User;
            var model = new TrainersPaymentListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url, user),
                _Title = user.FullNameFNF + "'s " + WebLocalizationKeys.PAYMENT_AMOUNT,
                TrainersName = user.FullNameFNF,
                EntityId = user.EntityId,
                PayTrainerUrl = UrlContext.GetUrlForAction<TrainerSessionVerificationController>(x => x.Accept(null), AreaName.Billing),
            };
            return new CustomJsonResult { Data = model };
        }

        public JsonResult TrainerSessions(TrainerPaymentGridItemsRequestModel input)
        {
            var user = ((User) input.User);
            var sessions = user.Sessions.Where(x => !x.TrainerPaid).OrderBy(x=>x.InArrears).ThenBy(x=>x.Client.LastName).ThenBy(x=>x.Appointment.Date);
            var endDate = input.endDate.HasValue ? input.endDate : DateTime.Now;
            var items = _dynamicExpressionQuery.PerformQuery(sessions,input.filters, x=>x.Appointment.Date<=endDate);
            var sessionPaymentDtos = items.Select(x => new SessionVerificationDto
                                                           {
                                                               AppointmentDate = x.Appointment.Date,
                                                               EntityId = x.EntityId,
                                                               FullName = x.Client.FullNameLNF,
                                                               PricePerSession = x.Cost,
                                                               Type = x.AppointmentType,
                                                               TrainerPercentage =
                                                                   x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client)!=null?x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client).Percent:x.Trainer.ClientRateDefault,
                                                               TrainerPay =
                                                                   x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client)!=null?x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client).Percent * .01 * x.Cost : x.Trainer.ClientRateDefault * .01 * x.Cost
                                                           });


            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, sessionPaymentDtos,user);
            return new CustomJsonResult { Data = gridItemsViewModel };
        }

        public CustomJsonResult Accept(PayTrainerViewModel input)
        {
            Notification notification =null;
//            var trainerPayment = trainer.PayTrainer(input.eligableRows, input.paymentAmount);
//            if (trainerPayment == null)
//            {
//                notification = new Notification { Success = false, Message = WebLocalizationKeys.YOU_MUST_SELECT_AT_LEAST_ONE_SESSION.ToString() };
//                return new CustomJsonResult { Data = notification };
//            }
//            var crudManager = _saveEntityService.ProcessSave(trainer);
//            notification = crudManager.Finish();
//            notification.Variable = UrlContext.GetUrlForAction<PayTrainerController>(x => x.TrainerReceipt(null), AreaName.Billing) + "/" + trainerPayment.EntityId + "?ParentId=" + trainer.EntityId;
            return new CustomJsonResult { Data = notification };
        }

    }

    public class SessionVerificationDto : IGridEnabledClass
    {
        public int EntityId { get; set; }
        public string FullName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Type { get; set; }
        public double PricePerSession { get; set; }
        public int TrainerPercentage { get; set; }
        public double TrainerPay { get; set; }
    }
}
