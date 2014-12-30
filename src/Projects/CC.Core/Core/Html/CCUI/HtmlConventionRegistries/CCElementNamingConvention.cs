using System;
using CC.Core.Reflection;
using CC.Core.UI.Helpers.Configuration;

namespace CC.Core.Core.Html.CCUI.HtmlConventionRegistries
{
    public class CCElementNamingConvention : IElementNamingConvention
    {
        public string GetName(Type modelType, Accessor accessor)
        {
            return accessor.FieldName;
        }
    }
}