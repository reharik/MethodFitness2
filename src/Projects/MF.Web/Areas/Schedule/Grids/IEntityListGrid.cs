﻿using System;
using System.Linq;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.Html.Grid;
using CC.Security;

namespace MF.Web.Areas.Schedule.Grids
{
    public interface IEntityListGrid<ENTITY> where ENTITY : IGridEnabledClass
    {
        void AddColumnModifications(Action<IGridColumn, ENTITY> modification);
        GridDefinition GetGridDefinition(string url, IUser user);
        GridItemsViewModel GetGridItemsViewModel(PageSortFilter pageSortFilter, IQueryable<ENTITY> items, IUser user);
    }
}