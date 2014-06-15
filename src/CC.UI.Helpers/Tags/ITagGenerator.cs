using System;
using System.Linq.Expressions;
using CC.Core.Utilities;
using CC.UI.Helpers.Configuration;
using HtmlTags;

namespace CC.UI.Helpers.Tags
{
    public interface ITagGenerator<T> where T : class
    {
        string ElementPrefix { get; set; }

        string CurrentProfile { get; }

        T Model { get; set; }

        void SetProfile(string profileName);

        HtmlTag LabelFor(Expression<Func<T, object>> expression);

        HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile);

        HtmlTag InputFor(Expression<Func<T, object>> expression);

        HtmlTag InputFor(Expression<Func<T, object>> expression, string profile);

        HtmlTag DisplayFor(Expression<Func<T, object>> expression);

        HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile);

        ElementRequest GetRequest(Expression<Func<T, object>> expression);

        HtmlTag LabelFor(ElementRequest request);

        HtmlTag InputFor(ElementRequest request);

        HtmlTag DisplayFor(ElementRequest request);

        ElementRequest GetRequest<TProperty>(Expression<Func<T, TProperty>> expression);

        ElementRequest GetRequest(Accessor accessor);
    }
}