using MethodFitness.Core.Domain;
using MethodFitness.Core.Html.Grid;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;

namespace MethodFitness.Web.Grids
{
    public class TrainerPaymentListGrid : Grid<TrainerPayment>, IEntityListGrid<TrainerPayment>
    {

        public TrainerPaymentListGrid(IGridBuilder<TrainerPayment> gridBuilder,
                                      ISessionContext sessionContext,
                                      IRepository repository)
            : base(gridBuilder, sessionContext, repository)
        {
        }

        protected override Grid<TrainerPayment> BuildGrid()
        {
            GridBuilder.LinkColumnFor(x => x.CreateDate)
                .ToPerformAction(ColumnAction.DisplayItem).WithId("trainerPaymentsList")
                .ToolTip(WebLocalizationKeys.DISPLAY_ITEM)
                .DefaultSortColumn().SecurityOperation("/TrainerPayment/Display");
            GridBuilder.DisplayFor(x => x.Total);
            return this;
        }
    }
}