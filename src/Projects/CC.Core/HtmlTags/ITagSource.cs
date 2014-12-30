using System.Collections.Generic;

namespace CC.Core.HtmlTags
{
    public interface ITagSource
    {
        IEnumerable<HtmlTag> AllTags();
    }
}