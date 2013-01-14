﻿using CC.Core.DomainTools;
using CC.Core.Html.Grid;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Schedule.Grids
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
                .ToPerformAction(ColumnAction.Redirect).ImageName("pay_trainer.png");
            GridBuilder.SetSearchField(x => x.LastName);
            GridBuilder.SetDefaultSortColumn(x => x.LastName);

            return this;
        }
    }
}