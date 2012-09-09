using System;
using System.Collections.Generic;
using FubuMVC.UI.Configuration;
using HtmlTags;
using MethodFitness.Core.Html.FubuUI.HtmlConventionRegistries;

namespace MethodFitness.Core.Html.FubuUI.Builders
{
    public class ListDisplayBuilder : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return def.Accessor.PropertyType == typeof (IEnumerable<string>);
        }

        public override HtmlTag Build(ElementRequest request)
        {
            var ul = new HtmlTag("ul").Attr("data-bind", "foreach:" + MethodFitnessHtmlConventions2.DeriveElementName(request));
            var li = new HtmlTag("li");
            li.Children.Add(new HtmlTag("span").Attr("data-bind", "text:$data"));
            ul.Children.Add(li);
            return ul;
        }
    }
}