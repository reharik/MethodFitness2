using CC.Core.Core.Html.Grid;
using MF.Core.Domain;
using MF.Web.Areas.Billing.Controllers;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Controllers;

namespace MF.Web.Grids
{
    public class ClientListGrid : Grid<Client>, IEntityListGrid<Client>
    {
        public ClientListGrid(IGridBuilder<Client> gridBuilder)
            : base(gridBuilder)
        {
        }

        protected override Grid<Client> BuildGrid()
        {
            GridBuilder.LinkColumnFor(x => x.LastName)
                .ForAction<ClientController>(x => x.AddUpdate(null))
                .ToPerformAction(ColumnAction.AddUpdateItem)
                .ToolTip(WebLocalizationKeys.EDIT_ITEM);
            GridBuilder.DisplayFor(x => x.FirstName);
            GridBuilder.DisplayFor(x => x.Email);
            GridBuilder.DisplayFor(x => x.MobilePhone);
            GridBuilder.ImageButtonColumn().ForAction<PaymentListController>(x => x.ItemList(null))
                .ToPerformAction(ColumnAction.Redirect).ImageName("client_payment.png");
            GridBuilder.SetSearchField(x => x.LastName);
            GridBuilder.SetDefaultSortColumn(x => x.LastName);
            GridBuilder.LinkColumnFor(x => x.Archived)
                .ToPerformAction(ColumnAction.Other).DisplayHeader("Archive")
                .SecurityOperation("/ArchiveClient");

            return this;
        }
    }
}
