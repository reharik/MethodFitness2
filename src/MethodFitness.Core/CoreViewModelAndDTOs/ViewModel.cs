using MethodFitness.Core.Html.Grid;

namespace MethodFitness.Core
{
    public class ViewModel
    {
        public int EntityId { get; set; }
        public int ParentId { get; set; }
        public int RootId { get; set; }
        public string Title { get; set; }
    }

    public class ListViewModel :ViewModel
    {
        public string AddEditUrl { get; set; }
        public string DeleteMultipleUrl { get; set; }
        public GridDefinition GridDefinition { get; set; }
        public string DisplayUrl { get; set; }
    }

    public class AssetListViewModel : ListViewModel
    {
        public AssetListViewModel(ListViewModel parent)
        {
            AddEditUrl = parent.AddEditUrl;
            DeleteMultipleUrl = parent.DeleteMultipleUrl;
            GridDefinition = parent.GridDefinition;
            EntityId = parent.EntityId;
            ParentId = parent.ParentId;
            RootId = parent.RootId;
            Title = parent.Title;
        }

        public AssetListViewModel()
        {
        }

        public string DeleteUrl { get; set; }
        public string CopyItemsUrl { get; set; }
        public string AddItemsToPortfolioUrl { get; set; }
        public string HandleAddItemsToPortfolioUrl { get; set; }
        public string AddEditPortfolioUrl { get; set; }

        public string PortfolioListUrl { get; set; }
    }

   
}