using System;
using FubuMVC.UI.Configuration;
using HtmlTags;

namespace MethodFitness.Core.Html.FubuUI.HtmlConventionRegistries
{
    public static class CustomTagActionExpressions
    {
        public static HtmlTag BuildTextbox2(ElementRequest request)
        {
            var date = DateTime.Parse(request.StringValue()).ToShortDateString();
            return new TextboxTag().Attr("value", date).AddClass("mf_datePicker");
        }

    }
}