using System.Collections.Generic;
using CC.Core.Core.CoreViewModelAndDTOs;

namespace MF.Web.Areas.Schedule.Models.BulkAction
{
    public class BulkActionViewModel:ViewModel
    {
        public IEnumerable<int> EntityIds { get; set; }
    }
}