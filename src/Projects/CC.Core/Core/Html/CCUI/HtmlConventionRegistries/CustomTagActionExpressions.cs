using System;
using CC.Core.HtmlTags;
using CC.Core.UI.Helpers.Configuration;

namespace CC.Core.Core.Html.CCUI.HtmlConventionRegistries
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
