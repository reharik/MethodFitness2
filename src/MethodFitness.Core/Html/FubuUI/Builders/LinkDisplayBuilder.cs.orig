using CC.Core.Utilities;
using CCUIHelpers.Configuration;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using HtmlTags;

namespace MethodFitness.Core.Html.FubuUI.Builders
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