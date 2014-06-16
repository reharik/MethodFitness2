using System;
using CC.UI.Helpers.Configuration;
using HtmlTags;

namespace CC.Core.Html.CCUI.HtmlConventionRegistries
{
    public static class CustomTagActionExpressions
    {
        public static HtmlTag BuildTextbox2(ElementRequest request)
        {
            var date = DateTime.Parse(request.StringValue()).ToShortDateString();
            return new TextboxTag().Attr("value", date).AddClass("datePicker");
        }

    }
}
