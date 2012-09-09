using System;
using FubuMVC.Core.Util;
using FubuMVC.UI.Configuration;
using HtmlTags;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using MethodFitness.Core.Html.FubuUI.HtmlConventionRegistries;

namespace MethodFitness.Core.Html.FubuUI.Builders
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
            return new HtmlTag("textarea").Attr("data-bind", "value:" + MethodFitnessHtmlConventions2.DeriveElementName(request)).AddClass("textArea").Attr("name", request.ElementId);
        }
    }
}

