using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html.Grid;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Controllers;

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
                .ForAction<PaymentController>(x => x.Display(null), AreaName.Billing)
                .ToPerformAction(ColumnAction.DisplayItem)
                .ToolTip(WebLocalizationKeys.DISPLAY_ITEM)
                .DefaultSortColumn().SecurityOperation("/TrainerPayment/Display");
            GridBuilder.DisplayFor(x => x.Total);
            return this;
        }
    }
}