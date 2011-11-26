using System;
using System.Collections.Generic;
using FubuMVC.UI.Configuration;
using HtmlTags;

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
            HtmlTag root = new HtmlTag("div");
            var selectListItems = request.RawValue as IEnumerable<string>;
            if (selectListItems == null) return root;
            selectListItems.Each(item=>
                                     {
                                         root.Append(new HtmlTag("span").Text(item));
                                         root.Append(new HtmlTag("br"));
                                     });
            return root;
        }
    }
}