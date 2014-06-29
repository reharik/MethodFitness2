using CC.Core.Html.Grid;
using MF.Core.CoreViewModelAndDTOs;
using MF.Web.Areas.Schedule.Grids;

namespace MF.Web.Grids
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
            GridBuilder.HideColumnFor(x => x.InArrears);
            GridBuilder.SetSearchField(x => x.LastName);
            GridBuilder.SetDefaultSortColumn(x => x.LastName);

            return this;
        }
    }
}