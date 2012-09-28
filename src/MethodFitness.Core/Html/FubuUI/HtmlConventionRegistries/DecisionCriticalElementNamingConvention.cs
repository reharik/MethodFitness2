using System;
using CC.Core.Utilities;
using CCUIHelpers.Configuration;

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