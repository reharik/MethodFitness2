using System.Collections.Generic;
using CC.Core.Core.Html.CCUI.HtmlConventionRegistries;
using CC.Core.Core.Localization;
using CC.Core.HtmlTags;
using CC.Core.Reflection;
using CC.Core.UI.Helpers.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CC.Core.Core.Html.CCUI.Builders
{
    public class SelectFromEnumerationBuilder2 : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return def.Accessor.HasAttribute<ValueOfAttribute>();
        }

        public override HtmlTag Build(ElementRequest request)
        {
            var selectTag = new SelectTag();
            var elementName = CCHtmlConventions2.DeriveElementName(request);
            selectTag.Attr("data-bind", "options:_" + elementName + "List," +
                                        "optionsText:'Text'," +
                                        "optionsValue:'Value'," +
                                        "value:" + elementName );
            return selectTag;
        }
    }

    public class SelectFromIEnumerableBuilder2 : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            var propertyName = def.Accessor.FieldName;
            var listPropertyInfo = def.ModelType.GetProperty("_" + propertyName + "List");
            return (listPropertyInfo != null && listPropertyInfo.PropertyType == typeof(IEnumerable<SelectListItem>));
        }

        public override HtmlTag Build(ElementRequest request)
        {
            var selectTag = new SelectTag();
            var elementName = CCHtmlConventions2.DeriveElementName(request);
            var elementRoot = elementName.Contains("EntityId") ? elementName.Replace(".EntityId", "") : elementName;
            selectTag.Attr("data-bind", "options:_" + elementRoot +"List," +
                                        "optionsValue:'Value'," +
                                        "optionsText:'Text'," +
                                        "value:" + elementName);

            return selectTag;
        }
    }
}