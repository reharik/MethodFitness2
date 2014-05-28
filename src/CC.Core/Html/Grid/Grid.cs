using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.Domain;
using CC.Security;

namespace CC.Core.Html.Grid
{
    public interface IGrid<T> where T : IGridEnabledClass
    {
        void AddColumnModifications(Action<IGridColumn, T> modification);
        GridDefinition GetGridDefinition(string url, IUser user);
        GridItemsViewModel GetGridItemsViewModel(PageSortFilter pageSortFilter, IQueryable<T> items, IUser user);
    }

    public abstract class Grid<T> : IGrid<T> where T : IGridEnabledClass
    {
        protected readonly IGridBuilder<T> GridBuilder;
        private IList<Action<IGridColumn, T>> _modifications;

        protected Grid(IGridBuilder<T> gridBuilder)
        {
            GridBuilder = gridBuilder;
            _modifications = new List<Action<IGridColumn, T>>();
        }

        private IList<IDictionary<string, string>> GetGridColumns(IUser user)
        {
            return GridBuilder.ToGridColumns(user);
        }

        private IEnumerable GetGridRows(IEnumerable rawResults, IUser user)
        {
            foreach (T x in rawResults)
            {
                yield return new GridRow { id = x.EntityId, cell = GridBuilder.ToGridRow(x, user, _modifications) };
            }
        }

        public string GetDefaultSortColumnName()
        {
            var column = GridBuilder.columns.FirstOrDefault(
                x => x.Properties.Any(y => y.Key == GridColumnProperties.sortColumn.ToString()));
            return column == null ? string.Empty : column.Properties[GridColumnProperties.header.ToString()];
        }

        public void AddColumnModifications(Action<IGridColumn, T> modification)
        {
            _modifications.Add(modification);
        }


        public GridDefinition GetGridDefinition(string url, IUser user)
        {
            Grid<T> buildGrid = BuildGrid();
            return new GridDefinition
            {
                Url = url,
                Columns = buildGrid.GetGridColumns(user),
                SearchField = GridBuilder.GetSearchField(),
                DefaultSortColumn = GridBuilder.GetDefaultSortColumn()
            };
        }

        public GridItemsViewModel GetGridItemsViewModel(PageSortFilter pageSortFilter, IQueryable<T> items, IUser user)
        {
            var pager = new Pager<T>();
            var pageAndSort = pager.PageAndSort(items, pageSortFilter);
            var model = new GridItemsViewModel
            {
                items = BuildGrid().GetGridRows(pageAndSort.Items, user),
//                page = pageAndSort.Page,
                records = pageAndSort.TotalRows,
//                total = pageAndSort.TotalPages
            };
            return model;
        }

        protected virtual Grid<T> BuildGrid()
        {
            return this;
        }
    }

    public interface IGridEnabledClass:IReadableObject
    {
        int EntityId { get; set; }
    }

    public class GridRow
    {
        public int id { get; set; }
        public string[] cell { get; set; }
    }
}