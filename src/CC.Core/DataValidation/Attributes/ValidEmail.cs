using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CC.Core.DataValidation.Attributes
{
    public class ValidEmailAttribute:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            Regex regex = new Regex("^([a-zA-Z0-9_\\-\\.\\'\\+]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", RegexOptions.IgnoreCase );

            Match match = regex.Match(value.ToString());

            ErrorMessage = !match.Success ? "Must be a valid Email Address" : string.Empty;

            return match.Success;
        } 
    }
}