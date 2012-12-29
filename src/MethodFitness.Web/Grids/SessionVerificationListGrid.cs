using CC.Core.DomainTools;
using CC.Core.Html.Grid;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Areas.Schedule.Grids;

namespace MethodFitness.Web.Grids
{
    public class SessionVerificationListGrid : Grid<TrainerSessionDto>, IEntityListGrid<TrainerSessionDto>
    {

        public SessionVerificationListGrid(IGridBuilder<TrainerSessionDto> gridBuilder)
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
            return this;
        }
    }

    
}