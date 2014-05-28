using HtmlTags;

namespace CC.UI.Helpers.Configuration
{
    public abstract class ElementBuilder : IElementBuilder
    {
        #region IElementBuilder Members

        public TagBuilder CreateInitial(AccessorDef accessorDef)
        {
            if (matches(accessorDef))
                return Build;
            else
                return null;
        }

        #endregion

        protected abstract bool matches(AccessorDef def);

        public abstract HtmlTag Build(ElementRequest request);
    }
}