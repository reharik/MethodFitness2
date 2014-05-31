using HtmlTags;

namespace CC.Core.Html.CCUI.Tags
{
    public class PasswordTag : HtmlTag
    {
        public PasswordTag() : base("input")
        {
            Attr("type", "password");
        }

        public PasswordTag(string name, string value)
            : this()
        {
            Attr("name", name);
            Attr("value", value);
        }

    }
}