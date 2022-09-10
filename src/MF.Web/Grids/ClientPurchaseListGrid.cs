using CC.Core.Core.Html.Grid;
using MF.Core.Domain;
using MF.Web.Areas.Schedule.Grids;

namespace MF.Web.Grids
{
    public class ClientPurchaseListGrid : Grid<Payment>, IEntityListGrid<Payment>
    {

        public ClientPurchaseListGrid(IGridBuilder<Payment> gridBuilder)
            : base(gridBuilder)
        {
        }

        protected override Grid<Payment> BuildGrid()
        {
            GridBuilder.LinkColumnFor(x => x.CreatedDate)
                .ToPerformAction(ColumnAction.AddUpdateItem)
                .ToolTip(WebLocalizationKeys.EDIT_ITEM)
                .SecurityOperation("/Payment/AddUpdate");
            GridBuilder.LinkColumnFor(x => x.CreatedDate)
                .ToPerformAction(ColumnAction.DisplayItem)
                .ToolTip(WebLocalizationKeys.DISPLAY_ITEM)
                .SecurityOperation("/Payment/Display");
            GridBuilder.DisplayFor(x => x.PaymentTotal);
            GridBuilder.SetSearchField(x => x.CreatedDate);
            GridBuilder.SetDefaultSortColumn(x => x.CreatedDate);

            return this;
        }
    }
}