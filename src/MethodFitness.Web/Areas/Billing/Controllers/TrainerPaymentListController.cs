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
        private readonly IEntityListGrid<SessionPaymentDto> _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;

        public TrainerPaymentListController(IEntityListGrid<SessionPaymentDto> grid, IDynamicExpressionQuery dynamicExpressionQuery, IRepository repository)
        {
            _grid = grid;
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _repository = repository;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<TrainerPaymentListController>(x => x.TrainerPayments(null),AreaName.Billing) + "?ParentId="+input.EntityId;
            var model = new ListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url)
            };
            return Json(model,JsonRequestBehavior.AllowGet);
        }

        public JsonResult TrainerPayments(GridItemsRequestModel input)
        {
            var trainer = _repository.Find<User>(input.ParentId);
            var sessions = trainer.Sessions.Where(x => !x.TrainerPaid);
            var items = _dynamicExpressionQuery.PerformQueryWithItems(sessions,input.filters);
            var sessionPaymentDtos = items.Select(x => new SessionPaymentDto
                                                           {
                                                               AppointmentDate = x.Appointment.Date,
                                                               EntityId = x.EntityId,
                                                               FullName = x.Client.FullNameLNF,
                                                               PricePerSession = x.Cost,
                                                               Type = x.AppointmentType,
                                                               TrainerPercentage =
                                                                   x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client).Percent,
                                                               TrainerPay =
                                                                   x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client).Percent*.01*x.Cost
                                                           });


            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, sessionPaymentDtos);
            return Json(gridItemsViewModel, JsonRequestBehavior.AllowGet);
        }
    }



    public class SessionPaymentDto : IGridEnabledClass
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