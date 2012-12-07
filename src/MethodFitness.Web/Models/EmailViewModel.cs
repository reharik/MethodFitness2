namespace MethodFitness.Web.Models
{
    using CC.Core.CustomAttributes;

    using Castle.Components.Validator;

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