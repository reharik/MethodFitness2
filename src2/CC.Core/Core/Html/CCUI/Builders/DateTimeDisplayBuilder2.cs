using System;
using CC.Core.Core.Html.CCUI.HtmlConventionRegistries;
using CC.Core.HtmlTags;
using CC.Core.UI.Helpers.Configuration;

namespace CC.Core.Core.Html.CCUI.Builders
{
    public class DateDisplayBuilder2 : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return (def.Accessor.PropertyType == typeof(DateTime)
                || def.Accessor.PropertyType == typeof(DateTime?))
                && !def.Accessor.FieldName.EndsWith("Time");
        }

        public override HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("span").Attr("data-bind", "dateString:" + CCHtmlConventions2.DeriveElementName(request));
        }
    }

    public class TimeDisplayBuilder2 : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return (def.Accessor.PropertyType == typeof(DateTime)
                || def.Accessor.PropertyType == typeof(DateTime?))
                && def.Accessor.FieldName.EndsWith("Time");
        }

        public override HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("span").Attr("data-bind", "timeString:" + CCHtmlConventions2.DeriveElementName(request));
        }
    }
}