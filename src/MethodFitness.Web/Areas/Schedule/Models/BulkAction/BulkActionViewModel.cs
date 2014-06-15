using System.Collections.Generic;
using CC.Core.CoreViewModelAndDTOs;
using MethodFitness.Core;

namespace MethodFitness.Web.Areas.Portfolio.Models.BulkAction
{
    public class BulkActionViewModel:ViewModel
    {
        public IEnumerable<int> EntityIds { get; set; }
    }
}