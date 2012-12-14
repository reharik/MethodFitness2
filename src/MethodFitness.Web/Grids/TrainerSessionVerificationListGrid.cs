using CC.Core.Html.Grid;
using MethodFitness.Core.Domain;
using MethodFitness.Web.Areas.Schedule.Grids;

namespace MethodFitness.Web.Grids
{
    public class TrainerSessionVerificationListGrid : Grid<TrainerSessionVerification>, IEntityListGrid<TrainerSessionVerification>
    {

        public TrainerSessionVerificationListGrid(IGridBuilder<TrainerSessionVerification> gridBuilder)
            : base(gridBuilder)
        {
        }

        protected override Grid<TrainerSessionVerification> BuildGrid()
        {
            GridBuilder.LinkColumnFor(x => x.CreatedDate)
                .ToPerformAction(ColumnAction.DisplayItem).WithId("trainerPaymentsList")
                .ToolTip(WebLocalizationKeys.DISPLAY_ITEM)
                .DefaultSortColumn();
            GridBuilder.DisplayFor(x => x.Total);
            return this;
        }
    }
}