using System.Collections.Generic;
using CC.Core.Core.Html.CCUI.HtmlConventionRegistries;
using CC.Core.HtmlTags;
using CC.Core.UI.Helpers.Configuration;

namespace CC.Core.Core.Html.CCUI.Builders
{
    public class ListDisplayBuilder : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return def.Accessor.PropertyType == typeof (IEnumerable<string>);
        }

        public override HtmlTag Build(ElementRequest request)
        {
            var ul = new HtmlTag("ul").Attr("data-bind", "foreach:" + CCHtmlConventions2.DeriveElementName(request));
            var li = new HtmlTag("li");
            li.Children.Add(new HtmlTag("span").Attr("data-bind", "text:$data"));
            ul.Children.Add(li);
            return ul;
        }
    }
}