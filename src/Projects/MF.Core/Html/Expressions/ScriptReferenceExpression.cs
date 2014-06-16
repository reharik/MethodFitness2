using CC.Core;
using CC.Core.Html;
using HtmlTags;
using MF.Core.Domain;

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
          
            var fullUrl = UrlContext.Combine(_baseUrl, _fileName).ToFullUrl();
            return new HtmlTag("script").Attr("src", fullUrl).Attr("type", "text/javascript");
        }
    }
}