using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Html.Grid;
using CC.Core.Services;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;
using NHibernate.Linq;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class PayTrainerListController : MFController
    {
        private readonly IEntityListGrid<SessionPaymentDto> _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;

        public PayTrainerListController(IEntityListGrid<SessionPaymentDto> grid,
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
            var user = _sessionContext.GetCurrentUser();
            var trainer = _repository.Find<User>(input.EntityId);
            var url = UrlContext.GetUrlForAction<PayTrainerListController>(x => x.TrainerPayments(null),AreaName.Billing) + "?ParentId="+input.EntityId;
            var model = new TrainersPaymentListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url, user),
                _Title = trainer.FullNameFNF+"'s " + WebLocalizationKeys.PAYMENT_AMOUNT,
                TrainersName = trainer.FullNameFNF,
                EntityId = trainer.EntityId,
                PayTrainerUrl = UrlContext.GetUrlForAction<PayTrainerController>(x=>x.PayTrainer(null),AreaName.Billing),
            };
            model.headerButtons.Add("return");
            return new CustomJsonResult(model);
        }

        public JsonResult TrainerPayments(TrainerPaymentGridItemsRequestModel input)
        {
            var user = _sessionContext.GetCurrentUser();
            var trainer = _repository.Query<User>(x=>x.EntityId == input.ParentId).FetchMany(x=>x.Sessions).ThenFetch(x=>x.Appointment).ThenFetchMany(x=>x.Clients).FirstOrDefault();
            var sessions = trainer.Sessions.Where(x => !x.TrainerPaid).OrderBy(x=>x.InArrears).ThenBy(x=>x.Client.LastName).ThenBy(x=>x.Appointment.Date);
            var endDate = input.endDate.HasValue ? input.endDate : DateTime.Now;
            var items = _dynamicExpressionQuery.PerformQuery(sessions,input.filters, x=>x.Appointment.Date<=endDate);
            var sessionPaymentDtos = items.Select(x => new SessionPaymentDto
                                                           {
                                                               AppointmentDate = x.Appointment.Date,
                                                               EntityId = x.EntityId,
                                                               FullName = x.Client.FullNameLNF,
                                                               PricePerSession = x.Cost,
                                                               InArrears = x.InArrears,
                                                               TrainerVerified = x.TrainerVerified,
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
            return new CustomJsonResult(gridItemsViewModel);
        }
    }

    public class TrainerPaymentGridItemsRequestModel : GridItemsRequestModel
    {
        public DateTime? endDate { get; set; }
    }

    public class PayTrainerViewModel:ViewModel
    {
        public double paymentAmount { get; set; }
        public IEnumerable<PaymentDetailsDto> eligableRows { get; set; }
    }

    

    public class TrainersPaymentListViewModel:ListViewModel
    {
        public string TrainersName { get; set; }
        public string PayTrainerUrl { get; set; }
        public string HeaderHtml { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string AlertAdminEmailUrl { get; set; }

        public string AcceptSessionsUrl { get; set; }
    }
}
