using System;
using System.Linq;
using HtmlTags;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Html.Grid;

namespace MethodFitness.Web.Areas.Schedule.Grids
{
    public interface IEntityListGrid<ENTITY> where ENTITY : IGridEnabledClass
    {
        void AddColumnModifications(Action<IGridColumn, ENTITY> modification);
        GridDefinition GetGridDefinition(string url);
        GridItemsViewModel  GetGridItemsViewModel(PageSortFilter pageSortFilter, IQueryable<ENTITY> items);
    }
}