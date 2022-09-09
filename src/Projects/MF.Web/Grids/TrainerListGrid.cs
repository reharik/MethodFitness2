using CC.Core.Core.Html.Grid;
using MF.Core.Domain;
using MF.Web.Areas.Billing.Controllers;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Controllers;

namespace MF.Web.Grids
{
    public class TrainerListGrid : Grid<User>, IEntityListGrid<User>
    {

        public TrainerListGrid(IGridBuilder<User> gridBuilder)
            : base(gridBuilder)
        {
        }

        protected override Grid<User> BuildGrid()
        {
            GridBuilder.LinkColumnFor(x => x.FirstName)
                       .ForAction<TrainerController>(x => x.AddUpdate(null))
                       .ToPerformAction(ColumnAction.AddUpdateItem)
                       .ToolTip(WebLocalizationKeys.EDIT_ITEM);
            GridBuilder.DisplayFor(x => x.LastName);
            GridBuilder.DisplayFor(x => x.Email);
            GridBuilder.DisplayFor(x => x.PhoneMobile);
            GridBuilder.ImageButtonColumn().ForAction<PayTrainerListController>(x => x.ItemList(null))
                .ToPerformAction(ColumnAction.Redirect).ImageName("pay_trainer.png").SecurityOperation("/PayTrainer"); ;
            GridBuilder.SetSearchField(x => x.LastName);
            GridBuilder.SetDefaultSortColumn(x => x.LastName);
            GridBuilder.LinkColumnFor(x => x.Archived)
                .ToPerformAction(ColumnAction.Other).DisplayHeader("Archive");
            return this;
        }
    }
}