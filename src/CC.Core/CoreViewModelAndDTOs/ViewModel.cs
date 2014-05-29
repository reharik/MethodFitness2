using System.Collections.Generic;
using CC.Core.Html.Grid;
using CC.Security;

namespace CC.Core.CoreViewModelAndDTOs
{
    public class ViewModel
    {
        public IUser User { get; set; }
        public int EntityId { get; set; }
        public int ParentId { get; set; }
        public int RootId { get; set; }
        public string _Title { get; set; }
        public string addUpdateUrl { get; set; }
        public string displayUrl { get; set; }
        public string _saveUrl { get; set; }
        public string DateCreated { get; set; }
        public string Var { get; set; }
        public bool Popup { get; set; }
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