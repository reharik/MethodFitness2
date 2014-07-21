using CC.Core.CustomAttributes;

namespace CC.Core.DomainTools
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class TextAreaConvention :System.Attribute
    {
        //Make sure the properties with the [TextArea] attribute will be nvarChar(MAX) in the database
//        protected override void Apply(TextAreaAttribute attribute, IPropertyInstance instance)
//        {
//            instance.Length(10000);
//        }
    }
}