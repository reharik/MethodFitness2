using System;
using FubuMVC.Core.Util;
using FubuMVC.UI.Configuration;

namespace MethodFitness.Core.Html.FubuUI.HtmlConventionRegistries
{
    public class MethodFitnessElementNamingConvention : IElementNamingConvention
    {
        public string GetName(Type modelType, Accessor accessor)
        {
            return accessor.FieldName;
        }
    }
}