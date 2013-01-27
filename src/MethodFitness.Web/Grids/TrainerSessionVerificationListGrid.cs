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
                .ToPerformAction(ColumnAction.DisplayItem)  
                .ToolTip(WebLocalizationKeys.DISPLAY_ITEM);
            GridBuilder.DisplayFor(x => x.Total);
            GridBuilder.SetSearchField(x => x.CreatedDate);
            GridBuilder.SetDefaultSortColumn(x => x.CreatedDate);

            return this;
        }
    }
}