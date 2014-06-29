using CC.Core.CustomAttributes;
using CC.Core.Reflection;
using CC.UI.Helpers.Configuration;
using CC.Utility;
using HtmlTags;

namespace CC.Core.Html.CCUI.Builders
{
    public class TextAreaBuilder : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return (def.Accessor.PropertyType == typeof (string)
                    && def.Accessor.HasAttribute<TextAreaAttribute>());
        }

        public override HtmlTag Build(ElementRequest request)
        {
            var value = request.StringValue().IsNotEmpty() ? request.StringValue() : "";
            return new HtmlTag("textarea")
            .AddClasses("textArea")
                .Attr("name", request.ElementId)
                .Text(value);
        }
    }
}
    
