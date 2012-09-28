using System;
using System.Linq;
using System.Linq.Expressions;
using CC.Core.Utilities;
using MethodFitness.Core.Localization;

namespace MethodFitness.Core.Html.Grid
{
    public class DisplayColumn<ENTITY> : ColumnBase<ENTITY> where ENTITY : IGridEnabledClass 
    {
        public DisplayColumn(Expression<Func<ENTITY, object>> expression)
        {
            propertyAccessor = ReflectionHelper.GetAccessor(expression);
            var name = LocalizationManager.GetLocalString(expression);
            if(propertyAccessor is PropertyChain)
            {
                name = ((PropertyChain)(propertyAccessor)).PropertyNames.Aggregate((current, next) => current + "." + next);
            }
            Properties[GridColumnProperties.name.ToString()] = name;
            Properties[GridColumnProperties.header.ToString()] = LocalizationManager.GetHeader(expression).HeaderText;
        }

        public DisplayColumn<ENTITY> FormatValue(GridColumnFormatter formatter)
        {
            Properties[GridColumnProperties.formatter.ToString()] = formatter.ToString().ToLowerInvariant();
            return this;
        }

        public DisplayColumn<ENTITY> FormatOptions(GridColumnFormatterOptions option)
        {
            Properties[GridColumnProperties.formatoptions.ToString()] = option.ToString().ToLowerInvariant();
            return this;
        }
    }
}