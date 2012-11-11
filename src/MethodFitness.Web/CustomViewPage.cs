﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using CC.Core;
using CC.Core.Html.Menu;
using CC.Core.Localization;
using CC.Core.Utilities;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html.Expressions;

namespace MethodFitness.Web
{
    public abstract class CustomWebViewPage<T> : WebViewPage<T>
    {
        public static LinkExpression LinkTag()
        {
            return new LinkExpression();
        }

        public static LinkExpression CSS(string url)
        {
            return new LinkExpression().Href(url).AsStyleSheet();
        }

        public static ScriptReferenceExpression Script(string url)
        {
            return new ScriptReferenceExpression(url);
        }

        public static string ActionUrl<CONTROLLER>(Expression<Func<CONTROLLER, object>> actionExpression, AreaName area = null)
            where CONTROLLER : class
        {
            var accessor = ReflectionHelper.GetMethod(actionExpression);
            var action = accessor.Name;
            var controller = accessor.DeclaringType.Name.Replace("Controller", "");
            var areaName = area != null ? area.Key + "/" : "";

            return ("~/" + areaName + controller + "/" + action).ToFullUrl();
        }

//        public static FormExpression FormFor<CONTROLLER>(Expression<Func<CONTROLLER, object>> actionExpression, AreaName area = null, string id = null)
//            where CONTROLLER : class
//        {
//            var actionUrl = ActionUrl(actionExpression, area);
//            return new FormExpression(actionUrl,id);
//        }
//
//        public static FormExpression FormFor(string actionUrl, string id = null)
//        {
//            return new FormExpression(actionUrl, id);
//        }

        public static StandardButtonExpression StandardButtonFor(string name, string value)
        {
            return new StandardButtonExpression(name).NonLocalizedText(value);
        }

        public static StandardButtonExpression StandardButtonFor(string name, StringToken text)
        {
            return new StandardButtonExpression(name).LocalizedText(text);
        }


        /// <param name="collectionName">Collection name on model which contains selectable items</param>
        /// <param name="templateName">Template name for partial code</param>
      
        public MvcHtmlString EndForm()
        {
            return MvcHtmlString.Create("</form>");
        }

        public static MenuExpression MenuItems(IList<MenuItem> items)
        {
            return new MenuExpression(items);
        }

     
    }
}
