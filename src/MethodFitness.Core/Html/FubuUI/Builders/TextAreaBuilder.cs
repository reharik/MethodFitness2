using MethodFitness.Core.Domain.Tools.CustomAttributes;
using FubuMVC.Core.Util;
    using FubuMVC.UI.Configuration;
    using HtmlTags;

namespace MethodFitness.Core.Html.FubuUI.Builders
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
    
