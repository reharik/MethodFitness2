using System.Collections.Generic;
using CCUIHelpers.Configuration;
using HtmlTags;
using MethodFitness.Core.Html.FubuUI.HtmlConventionRegistries;

namespace MethodFitness.Core.Html.FubuUI.Builders
{
    public class ListDisplayBuilder2 : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return def.Accessor.PropertyType == typeof (IEnumerable<string>);
        }

        public override HtmlTag Build(ElementRequest request)
        {
            HtmlTag root = new HtmlTag("div").Attr("data-bind", "foreach: "+ MethodFitnessHtmlConventions2.DeriveElementName(request));
            var child = new HtmlTag("div").Attr("data-bind", "text: $data" );
            root.Append(child);
            return root;
        }
    }
}