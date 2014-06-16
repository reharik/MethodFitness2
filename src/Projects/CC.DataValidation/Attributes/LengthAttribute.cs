using System.ComponentModel.DataAnnotations;
using CC.Utility;

namespace CC.DataValidation.Attributes
{
    public class LengthRangeAttribute:ValidationAttribute
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public LengthRangeAttribute(int min, int max)
        {
            MinLength = min;
            MaxLength = max;
        }

        public override bool IsValid(object value)
        {
            if (value == null) { return true; }
            var val = value as string;
            var isValid = val.IsNotEmpty() || (val.Length >= MinLength && val.Length <= MaxLength);
            ErrorMessage = !isValid ? "Must be between {0} and {1} charicters long".ToFormat(MinLength,MaxLength) : string.Empty;
            return isValid;
        }
    }
}