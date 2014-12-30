using CC.Core.Core.CustomAttributes;
using CC.Core.Core.Html.CCUI.HtmlConventionRegistries;
using CC.Core.HtmlTags;
using CC.Core.Reflection;
using CC.Core.UI.Helpers.Configuration;

namespace CC.Core.Core.Html.CCUI.Builders
{
    public class TextAreaBuilder2 : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return (def.Accessor.PropertyType == typeof(string)
                    && def.Accessor.HasAttribute<TextAreaAttribute>());
        }

        public override HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("textarea").Attr("data-bind", "value:" + CCHtmlConventions2.DeriveElementName(request)).AddClass("textArea").Attr("name", request.ElementId);
        }
    }
}

