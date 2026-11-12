namespace CC.Core.DataValidation.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class DoNotValidateAttribute : System.Attribute
    {
        public DoNotValidateAttribute()
        {
            // used just as a flag for ui gen        
        }
    }
}