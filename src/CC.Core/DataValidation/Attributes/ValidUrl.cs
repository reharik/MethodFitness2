using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CC.Core.DataValidation.Attributes
{
    public class ValidUrlAttribute:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            Regex regex = new Regex(@"(([\w]+:)?//)?(([\d\w]|%[a-fA-f\d]{2,2})+(:([\d\w]|%[a-fA-f\d]{2,2})+)?@)?([\d\w][-\d\w]{0,253}[\d\w]\.)+[\w]{2,4}(:[\d]+)?(/([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)*(\?(&?([-+_~.\d\w]|%[a-fA-f\d]{2,2})=?)*)?(#([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            Match match = regex.Match(value.ToString());

            ErrorMessage = !match.Success ? "Must be a valid Url" : string.Empty;

            return match.Success;
        } 

    }
}