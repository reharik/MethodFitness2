using CC.Core.Html.Grid;
using MF.Core.Domain;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Controllers;
using MethodFitness.Web;

namespace MF.Web.Grids
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