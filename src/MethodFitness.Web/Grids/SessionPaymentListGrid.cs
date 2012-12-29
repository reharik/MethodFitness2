using CC.Core.DomainTools;
using CC.Core.Html.Grid;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Areas.Schedule.Grids;

namespace MethodFitness.Web.Grids
{
    public class SessionPaymentListGrid : Grid<TrainerSessionDto>, IEntityListGrid<TrainerSessionDto>
    {

        public SessionPaymentListGrid(IGridBuilder<TrainerSessionDto> gridBuilder)
            : base(gridBuilder)
        {
        }

        protected override Grid<TrainerSessionDto> BuildGrid()
        {
            GridBuilder.DisplayFor(x => x.FullName);
            GridBuilder.DisplayFor(x => x.AppointmentDate);
            GridBuilder.DisplayFor(x => x.Type);
            GridBuilder.DisplayFor(x => x.PricePerSession);
            GridBuilder.DisplayFor(x => x.TrainerPercentage);
            GridBuilder.DisplayFor(x => x.TrainerPay);
            GridBuilder.HideColumnFor(x => x.InArrears);
            GridBuilder.HideColumnFor(x => x.TrainerVerified);
            return this;
        }
    }

    
}