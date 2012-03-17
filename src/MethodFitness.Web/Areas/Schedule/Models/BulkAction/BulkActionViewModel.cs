using System.Collections.Generic;
using MethodFitness.Core;

namespace MethodFitness.Web.Areas.Portfolio.Models.BulkAction
{
    public class BulkActionViewModel:ViewModel
    {
        public IEnumerable<int> EntityIds { get; set; }
    }
}