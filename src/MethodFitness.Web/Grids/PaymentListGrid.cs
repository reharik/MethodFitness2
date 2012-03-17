using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html.Grid;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Grids
{
    public class PaymentListGrid : Grid<Payment>, IEntityListGrid<Payment>
    {

        public PaymentListGrid(IGridBuilder<Payment> gridBuilder,
                                      ISessionContext sessionContext,
                                      IRepository repository)
            : base(gridBuilder, sessionContext, repository)
        {
        }

        protected override Grid<Payment> BuildGrid()
        {
            GridBuilder.LinkColumnFor(x => x.CreateDate)
                .ForAction<PaymentController>(x => x.AddUpdate(null),AreaName.Billing)
                .ToPerformAction(ColumnAction.AddUpdateItem)
                .ToolTip(WebLocalizationKeys.EDIT_ITEM)
                .DefaultSortColumn().SecurityOperation("/Payment/AddUpdate");
            GridBuilder.LinkColumnFor(x => x.CreateDate)
                .ForAction<PaymentController>(x => x.Display(null), AreaName.Billing)
                .ToPerformAction(ColumnAction.DisplayItem)
                .ToolTip(WebLocalizationKeys.DISPLAY_ITEM)
                .DefaultSortColumn().SecurityOperation("/Payment/Display");
            GridBuilder.DisplayFor(x => x.PaymentTotal);
            return this;
        }
    }
}