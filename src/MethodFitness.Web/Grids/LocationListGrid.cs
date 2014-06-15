using CC.Core.Html.Grid;
using MethodFitness.Core.Domain;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Grids
{
    public class LocationListGrid : Grid<Location>, IEntityListGrid<Location>
    {

        public LocationListGrid(IGridBuilder<Location> gridBuilder)
            : base(gridBuilder)
        {
        }

        protected override Grid<Location> BuildGrid()
        {
            GridBuilder.LinkColumnFor(x => x.Name)
                       .ForAction<LocationController>(x => x.AddUpdate(null))
                       .ToPerformAction(ColumnAction.AddUpdateItem)
                       .ToolTip(WebLocalizationKeys.EDIT_ITEM);
            GridBuilder.DisplayFor(x => x.Address1);
            return this;
        }
    }
}