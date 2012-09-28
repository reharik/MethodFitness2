using System;
using CCUIHelpers.Configuration;
using HtmlTags;
using MethodFitness.Core.Html.FubuUI.HtmlConventionRegistries;

namespace MethodFitness.Core.Html.FubuUI.Builders
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
            return new HtmlTag("span").Attr("data-bind", "dateString:" + MethodFitnessHtmlConventions2.DeriveElementName(request));
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
            return new HtmlTag("span").Attr("data-bind", "timeString:" + MethodFitnessHtmlConventions2.DeriveElementName(request));
        }
    }
}