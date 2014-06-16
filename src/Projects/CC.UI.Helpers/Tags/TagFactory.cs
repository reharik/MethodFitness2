using System;
using System.Collections.Generic;
using System.Linq;
using CC.Core.Utilities;
using CC.UI.Helpers.Configuration;
using HtmlTags;

namespace CC.UI.Helpers.Tags
{
    public class TagFactory
    {
        private readonly Cache<AccessorDef, Func<ElementRequest, HtmlTag>> _creators =
            new Cache<AccessorDef, Func<ElementRequest, HtmlTag>>();

        private readonly IList<IElementModifier> _modifiers = new List<IElementModifier>();
        private readonly IList<IElementBuilder> _sources = new List<IElementBuilder>();

        public TagFactory()
        {
            _creators.OnMissing = resolveCreator;
        }

        public void AddModifier(IElementModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void AddBuilder(IElementBuilder builder)
        {
            _sources.Add(builder);
        }

        private Func<ElementRequest, HtmlTag> resolveCreator(AccessorDef accessorDef)
        {
            TagBuilder initialCreator = null;

            _sources.FirstOrDefault(x =>
                {
                    var tagBuilder = x.CreateInitial(accessorDef);
                    if (tagBuilder == null) { return false; }
                    initialCreator = tagBuilder;
                    return true;
                });

            if (initialCreator == null)
            {
                throw new Exception(string.Format("Html Conventions have no tag builder for {0}.{1}",
                                                  accessorDef.ModelType.FullName, accessorDef.Accessor.Name));
            }

            TagModifier[] modifiers = (_modifiers).Select((x => x.CreateModifier(accessorDef))).Where((x => x != null)).ToArray();
            return (request =>
                {
                    HtmlTag tag = initialCreator(request);
                    foreach (TagModifier v in modifiers)
                    {
                        v(request, tag);
                    }
                    return tag;
                });
        }

        public HtmlTag Build(ElementRequest request)
        {
            return _creators[request.ToAccessorDef()](request);
        }

        public void Merge(TagFactory factory)
        {
            foreach (IElementBuilder elementBuilder in factory._sources)
            {
                _sources.Add(elementBuilder);
            }

            foreach (IElementModifier elementModifier in factory._modifiers)
            {
                _modifiers.Add(elementModifier);
            }
        }
    }
}