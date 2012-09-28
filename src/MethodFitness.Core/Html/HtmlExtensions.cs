using System.Web.Mvc;
using CC.Core.Utilities;
using MethodFitness.Core.Html.Expressions;

namespace MethodFitness.Core.Html
{
    public static class HtmlExtensions
    {
        public static LinkExpression LinkTag<T>(this ViewPage<T> viewPage)
        {
            return new LinkExpression();
        }

        public static LinkExpression CSS(this ViewPage viewPage, string url)
        {
            return new LinkExpression().Href(url).AsStyleSheet();
        }

        public static LinkExpression SiteCSS(this ViewMasterPage viewMasterPage, string url)
        {
            return new LinkExpression().Href(url).AsStyleSheet();
        }

        public static ScriptReferenceExpression Script(this ViewPage viewPage, string url)
        {
            return new ScriptReferenceExpression(url);
        }

        public static ScriptReferenceExpression SiteScript(this ViewMasterPage viewMasterPage, string url)
        {
            return new ScriptReferenceExpression(url);
        }
        
        
    }
}
