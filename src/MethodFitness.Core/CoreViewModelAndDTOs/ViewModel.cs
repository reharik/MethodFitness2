using System.Collections.Generic;
using MethodFitness.Core.Html.Grid;

namespace MethodFitness.Core
{
    public class ViewModel
    {
        public int EntityId { get; set; }
        public int ParentId { get; set; }
        public int RootId { get; set; }
        public string Title { get; set; }
        public string addUpdateUrl { get; set; }
    }

    public class ListViewModel :ViewModel
    {
        public ListViewModel()
        {
            headerButtons = new List<string>();
        }

        public string deleteMultipleUrl { get; set; }
        public GridDefinition gridDef { get; set; }
        public List<string> headerButtons { get; set; }
    }
}