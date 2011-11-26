using System.Collections.Generic;
using System.Web.Mvc;
using DecisionCritical.Core;
using DecisionCritical.Core.Domain;

namespace DecisionCritical.Web.Models
{
    public class GoalViewModel : ListViewModel
    {
        public Goal Goal { get; set; }
        public IEnumerable<SelectListItem> GoalStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityList { get; set; }
        public DocumentTokenViewModel DocumentTokenViewModel { get; set; }
    }
}