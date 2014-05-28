using CC.Core.CustomAttributes;
using CC.Core.Utilities;
using CC.UI.Helpers.Configuration;
using HtmlTags;

namespace CC.Core.Html.CCUI.Builders
{
    public class LinkDisplayBuilder : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return (def.Accessor.PropertyType == typeof(string)
                      && def.Accessor.HasAttribute<LinkDisplayAttribute>());
        }

        public override HtmlTag Build(ElementRequest request)
        {
            HtmlTag root = new HtmlTag("a");
            root.Attr("href", "#");
            root.Id(request.Accessor.FieldName);
            root.Append(new HtmlTag("span").Text(request.StringValue()));
            return root;
        }
    }
}