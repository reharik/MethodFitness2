﻿using System.Collections.Generic;
using CC.Core.Core.Html.Menu;
using CC.Core.Core.Localization;
using MF.Core.Html.Expressions;
using Microsoft.AspNetCore.Html;

namespace MF.Web
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

        public static StandardButtonExpression StandardButtonFor(string name, string value)
        {
            return new StandardButtonExpression(name).NonLocalizedText(value);
        }

        public static StandardButtonExpression StandardButtonFor(string name, StringToken text)
        {
            return new StandardButtonExpression(name).LocalizedText(text);
        }

        public HtmlString EndForm()
        {
            return HtmlString.Create("</form>");
        }

        public static MenuExpression MenuItems(IList<MenuItem> items)
        {
            return new MenuExpression(items);
        }


    }
}
