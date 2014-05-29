using CC.Core.CustomAttributes;
using CC.Core.Html.CCUI.HtmlConventionRegistries;
using CC.Core.Utilities;
using CC.UI.Helpers.Configuration;
using HtmlTags;

namespace CC.Core.Html.CCUI.Builders
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

