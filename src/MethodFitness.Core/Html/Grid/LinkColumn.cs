using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Localization;
using FubuMVC.Core.Util;
using HtmlTags;
using MethodFitness.Security.Interfaces;

namespace MethodFitness.Core.Html.Grid
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

        public LinkColumn(Expression<Func<ENTITY, object>> expression)
        {
            _divCssClasses = new List<string>();
            propertyAccessor = ReflectionHelper.GetAccessor(expression); 
            var name = LocalizationManager.GetLocalString(expression);
            if (propertyAccessor is PropertyChain)
            {
                name = ((PropertyChain)(propertyAccessor)).Names.Aggregate((current, next) => current + "." + next);
            }
            Properties[GridColumnProperties.name.ToString()] = name;
            
            var headerText = LocalizationManager.GetHeader(expression).HeaderText;
            if(headerText == "Name")
            {
                headerText = typeof (ENTITY).Name.ToSeperateWordsFromPascalCase() + " " + headerText;
            }
            Properties[GridColumnProperties.header.ToString()] = headerText;
        }

        public LinkColumn<ENTITY> ForAction<CONTROLLER>(Expression<Func<CONTROLLER, object>> expression, AreaName area = null) where CONTROLLER : Controller
        {
            var urlForAction = UrlContext.GetUrlForAction(expression,area);
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

        public override ColumnValueDto BuildColumn(object item, User user, IAuthorizationService _authorizationService)
        {
            var _item = (ENTITY)item;
            var valueDto = FormatValue(_item, user, _authorizationService);
            if (valueDto.HtmlTag == null || valueDto.HtmlTag.Text().IsEmpty()) return valueDto;
            addToolTipAndClasses(valueDto.HtmlTag);
            var anchor = buildAnchor(_item);
            var div = BuildDiv();
            div.Children.Add(valueDto.HtmlTag);
            anchor.Children.Add(div);
            valueDto.HtmlTag = anchor;
            return valueDto;
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

        private HtmlTag buildAnchor(ENTITY item)
        {
            var anchor = new HtmlTag("a");
            anchor.Attr("onclick", "MF.vent.trigger('" + _action + "'," + item.EntityId + ")");

            return anchor;
        }

        public ColumnBase<ENTITY> AddClassToSpan(string cssClass)
        {
            _divCssClasses.Add(cssClass);
            return this;
        }
    }
}