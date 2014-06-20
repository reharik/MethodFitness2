using CC.Core.Html.Grid;
using MF.Core.Domain;
using MF.Web.Areas.Schedule.Grids;
using MF.Web;

namespace MF.Web.Grids
{
    public class TrainerPaymentListGrid : Grid<TrainerPayment>, IEntityListGrid<TrainerPayment>
    {

        public TrainerPaymentListGrid(IGridBuilder<TrainerPayment> gridBuilder)
            : base(gridBuilder)
        {
        }

        protected override Grid<TrainerPayment> BuildGrid()
        {
            GridBuilder.LinkColumnFor(x => x.CreatedDate)
                .ToPerformAction(ColumnAction.DisplayItem).WithId("trainerPaymentsList")
                .ToolTip(WebLocalizationKeys.DISPLAY_ITEM)
                .SecurityOperation("/TrainerPayment/Display");
            GridBuilder.DisplayFor(x => x.Total);
            GridBuilder.SetSearchField(x => x.CreatedDate);
            GridBuilder.SetDefaultSortColumn(x => x.CreatedDate);
            return this;
        }
    }
}