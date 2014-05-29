using HtmlTags;

namespace CC.Core.Html.CCUI.Tags
{
    public class RadioButtonTag : HtmlTag

    {
            public RadioButtonTag(bool isChecked)
                : base("input")
            {
                Attr("type", "radio");
                if (isChecked)
                {
                    Attr("checked", "true");
                }
            }
        }
    }
