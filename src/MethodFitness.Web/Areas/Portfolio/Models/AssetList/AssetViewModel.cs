using System.Collections.Generic;
using System.Web.Mvc;
using DecisionCritical.Core;
using DecisionCritical.Core.Domain;

namespace DecisionCritical.Web.Models
{
    public class AssetListViewModel : ListViewModel
    {
        public Asset Asset { get; set; }
    }

    public class AssetListCollectionViewModel : ListViewModel
    {
        public List<Asset> Asset { get; set; }
    }

}