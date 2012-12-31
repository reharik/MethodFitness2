using CC.Core.DomainTools;
using CC.Core.Html.Grid;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Grids
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
                .ToolTip(WebLocalizationKeys.EDIT_ITEM)
                .DefaultSortColumn();
            GridBuilder.DisplayFor(x => x.FirstName);
            GridBuilder.DisplayFor(x => x.Email);
            GridBuilder.DisplayFor(x => x.MobilePhone);
            GridBuilder.ImageButtonColumn().ForAction<PaymentListController>(x => x.ItemList(null))
                .ToPerformAction(ColumnAction.Redirect).ImageName("client_payment.png");
            GridBuilder.SetSearchField(x => x.LastName);

            return this;
        }
    }
}
