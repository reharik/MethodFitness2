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
                .ToPerformAction(ColumnAction.DisplayItem)
                .ToolTip(WebLocalizationKeys.DISPLAY_ITEM);
            // no SecurityOperation: admins are denied "/Payment/Display" (it used to pick
            // between the edit and read-only links), and everyone gets the read-only screen now
            GridBuilder.DisplayFor(x => x.PaymentTotal);
            GridBuilder.SetSearchField(x => x.CreatedDate);
            GridBuilder.SetDefaultSortColumn(x => x.CreatedDate);

            return this;
        }
    }
}