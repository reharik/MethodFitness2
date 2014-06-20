using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using MF.Core.CoreViewModelAndDTOs;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Config;
using MF.Web.Controllers;
using MF.Web.Grids;
using MF.Web;

namespace MF.Web.Areas.Billing.Controllers
{
    public class PayTrainerListController : MFController
    {
        private readonly SessionPaymentListGrid _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;

        public PayTrainerListController(SessionPaymentListGrid grid,
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
//            var trainer = _repository.Query<User>(x=>x.EntityId == input.ParentId).FetchMany(x=>x.Sessions).ThenFetch(x=>x.Appointment).ThenFetchMany(x=>x.Clients).FirstOrDefault();
//            var sessions = trainer.Sessions.Where(x => !x.TrainerPaid).OrderBy(x=>x.InArrears).ThenBy(x=>x.Client.LastName).ThenBy(x=>x.Appointment.Date);
            var endDate = input.endDate.HasValue ? input.endDate : DateTime.Now;
            var trainerSessionDtos = _repository.Query<TrainerSessionDto>(x => x.TrainerId == input.ParentId && x.AppointmentDate <= endDate && !x.TrainerPaid);
            var items = _dynamicExpressionQuery.PerformQuery(trainerSessionDtos, input.filters);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items, user);
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
