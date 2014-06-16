using CC.Core.CustomAttributes;
using Castle.Components.Validator;

namespace MF.Web.Models
{
    public class EmailViewModel
    {
        [ValidateNonEmpty]
        public string From { get; set; }
        [ValidateNonEmpty]
        public string To { get; set; }
        [ValidateNonEmpty]
        public string Subject { get; set; }
        [TextArea]
        [ValidateNonEmpty]
        public string Body { get; set; } 
    }
}