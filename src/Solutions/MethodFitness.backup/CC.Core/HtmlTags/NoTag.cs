namespace CC.Core.HtmlTags
{
    public class NoTag : HtmlTag
    {
        public NoTag() : base("")
        {
            Render(false);
        }
    }
}