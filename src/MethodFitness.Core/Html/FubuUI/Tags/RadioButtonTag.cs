using HtmlTags;

namespace MethodFitness.Core.Html.FubuUI.Tags
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
