using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CC.Core.Core.Html.Menu;
using CC.Core.Core.Localization;
using CC.Core.Core.Services;
using CC.Core.HtmlTags;
using CC.Core.Reflection;
using CC.Core.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CC.Core.Core.Html.Grid
{
    public class LinkColumn<ENTITY> : ColumnBase<ENTITY> where ENTITY : IGridEnabledClass
    {
        protected List<string> _divCssClasses;
        private string _actionUrl;
        public string ActionUrl
        {
            get { return _actionUrl; }
            set { _actionUrl = value; }
        }

        private string _action;
        private string _gridName;
        private readonly SiteConfigurationBase _config;
        private List<TriggerValueDto<ENTITY>> _returnValueWithTriggerList;
        private string _id;

        public LinkColumn(Expression<Func<ENTITY, object>> expression, SiteConfigurationBase config, string gridName = "")
        {
            _id = "gridContainer";
            _gridName = gridName;
            _config = config;
            _divCssClasses = new List<string>();
            propertyAccessor = ReflectionHelper.GetAccessor(expression);
            string name;
            if (_displayValue.IsNotEmpty())
            {
                name = _displayValue;
            }
            else
            {
                name = LocalizationManager.GetLocalString(expression);
                if (propertyAccessor is PropertyChain)
                {
                    name = ((PropertyChain)(propertyAccessor)).PropertyNames.Aggregate((current, next) => current + "." + next);
                }
            }
            Properties[GridColumnProperties.name.ToString()] = name;

            var headerText = LocalizationManager.GetHeader(expression).HeaderText;
            if (headerText == "Name")
            {
                headerText = typeof(ENTITY).Name.ToSeperateWordsFromPascalCase() + " " + headerText;
            }
            Properties[GridColumnProperties.header.ToString()] = headerText;
        }
        //used for getting controller from a field value like "InstantiatingType"
        public LinkColumn<ENTITY> ForAction(Expression<Func<ENTITY, object>> expression, string actionName)
        {
            var controllerName = ReflectionHelper.GetAccessor(expression).FieldName + "Controller";
            var urlForAction = UrlContext.GetUrlForAction(controllerName, actionName);
            _actionUrl = urlForAction;
            return this;
        }

        public LinkColumn<ENTITY> ForAction<CONTROLLER>(Expression<Func<CONTROLLER, object>> expression) where CONTROLLER : Controller
        {
            var urlForAction = UrlContext.GetUrlForAction(expression);
            _actionUrl = urlForAction;
            return this;
        }

        public LinkColumn<ENTITY> ForAction(string controllerName, string actionName)
        {
            var urlForAction = UrlContext.GetUrlForAction(controllerName, actionName);
            _actionUrl = urlForAction;
            return this;
        }

        public LinkColumn<ENTITY> ToPerformAction(ColumnAction action)
        {
            _action = action.ToString();
            return this;
        }

        public LinkColumn<ENTITY> FormatValue(GridColumnFormatter formatter)
        {
            Properties[GridColumnProperties.formatter.ToString()] = formatter.ToString().ToLowerInvariant();
            return this;
        }

        public LinkColumn<ENTITY> FormatOptions(GridColumnFormatterOptions option)
        {
            Properties[GridColumnProperties.formatoptions.ToString()] = option.ToString().ToLowerInvariant();
            return this;
        }

        public override string BuildColumn(object item, string gridName = "")
        {
            // if a name is given in the controller it overrides the name given in the grid declaration
            if (gridName.IsNotEmpty()) _gridName = gridName;
            var _item = (ENTITY)item;
            var value = FormatValue(_item);
            if (value.IsEmpty()) return null;
            var span = new HtmlTag("span").Text(value);
            addToolTipAndClasses(span);
            var anchor = buildAnchor(_item, _returnValueWithTriggerList);
            anchor.AddClasses(new[] { "linkColumn", _action });

            var div = BuildDiv();
            div.Children.Add(span);
            anchor.Children.Add(div);
            return anchor.ToString();
        }


        protected DivTag BuildDiv()
        {
            var divTag = new DivTag("imageDiv");
            divTag.Attr("title", _toolTip);
            divTag.AddClasses(_divCssClasses);
            return divTag;
        }

        private void addToolTipAndClasses(HtmlTag span)
        {
            span.Attr("title", _toolTip);
            span.AddClasses(_divCssClasses);
        }

        private HtmlTag buildAnchor(ENTITY item, List<TriggerValueDto<ENTITY>> expressions = null)
        {
            var anchor = new HtmlTag("a");
            var extraValues = getCSVofExtraValues(item, expressions);
            var id = _id.IsNotEmpty() ? _id + ":" : "";
            anchor.Attr("onclick", _config.jsApplicationName + ".vent.trigger('" + id + _action + "'," + item.EntityId + extraValues + ")");
            return anchor;
        }
        private string getCSVofExtraValues(ENTITY item, IEnumerable<TriggerValueDto<ENTITY>> expressions)
        {
            if (expressions == null) return string.Empty;
            var values = new List<string>();
            expressions.ForEachItem(x => values.Add(getExtraValue(item, x)));
            return "," + values.Aggregate((s1, s2) => s1 + "," + s2);
        }

        private string getExtraValue(ENTITY item, TriggerValueDto<ENTITY> expression)
        {
            if (expression == null) return string.Empty;
            var propertyValue = ReflectionHelper.GetAccessor(expression.Expression).GetValue(item);
            if (expression.Formatter != null)
            {
                propertyValue = expression.Formatter(propertyValue as string);
            }
            return propertyValue != null ? "'" + propertyValue + "'" : string.Empty;
        }
        public ColumnBase<ENTITY> AddClassToSpan(string cssClass)
        {
            _divCssClasses.Add(cssClass);
            return this;
        }

        public ColumnBase<ENTITY> WithId(string id)
        {
            _id = id;
            return this;
        }

        public LinkColumn<ENTITY> ReturnValueWithTrigger(Expression<Func<ENTITY, object>> expression, Func<string, string> stringFormat = null)
        {
            if (_returnValueWithTriggerList == null) _returnValueWithTriggerList = new List<TriggerValueDto<ENTITY>>();
            _returnValueWithTriggerList.Add(new TriggerValueDto<ENTITY> { Expression = expression, Formatter = stringFormat });
            return this;
        }
    }
    public class TriggerValueDto<ENTITY> where ENTITY : IGridEnabledClass
    {
        public Expression<Func<ENTITY, object>> Expression { get; set; }
        public Func<string, string> Formatter { get; set; }
    }
}