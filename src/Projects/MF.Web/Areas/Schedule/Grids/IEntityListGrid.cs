using System;
using System.Linq;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.Html.Grid;
using CC.Core.Security;

namespace MF.Web.Areas.Schedule.Grids
{
    public interface IEntityListGrid<ENTITY> where ENTITY : IGridEnabledClass
    {
        void AddColumnModifications(Action<IGridColumn, ENTITY> modification);
        GridDefinition GetGridDefinition(string url, IUser user);
        GridItemsViewModel GetGridItemsViewModel(PageSortFilter pageSortFilter, IQueryable<ENTITY> items, IUser user);
    }
}