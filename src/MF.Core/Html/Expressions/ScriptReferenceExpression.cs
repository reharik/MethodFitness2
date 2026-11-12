using CC.Core.Core.Html;
using CC.Core.Core.Html.Menu;
using CC.Core.HtmlTags;
using MF.Core.Domain;
using MF.Core.Services;

namespace MF.Core.Html.Expressions
{
    public class ScriptReferenceExpression
    {
        private string _baseUrl;
        private string _indentation;
        private string _fileName;

        public ScriptReferenceExpression(string fileName)
        {
            _baseUrl = Site.Config.ScriptsPath;
            _indentation = "";
            _fileName = fileName;
        }

        public ScriptReferenceExpression Indent(string indentation)
        {
            _indentation = indentation;
            return this;
        }

        public ScriptReferenceExpression BasedAt(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        public HtmlTag ToHtmlTag(bool findMinJs = false)
        {

            var fullUrl = UrlContext.GetFullUrl(UrlContext.Combine(_baseUrl, _fileName));
            return new HtmlTag("script").Attr("src", fullUrl).Attr("type", "text/javascript");
        }
    }
}