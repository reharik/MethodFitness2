using System;
using CC.UI.Helpers.Configuration;
using HtmlTags;

namespace CC.UI.Helpers.Tags
{
    public class TagActionExpression
    {
        private readonly TagFactory _factory;
        private readonly Func<AccessorDef, bool> _matches;

        public TagActionExpression(TagFactory factory, Func<AccessorDef, bool> matches)
        {
            _factory = factory;
            _matches = matches;
        }

        private void registerBuilder(TagBuilder builder)
        {
            _factory.AddBuilder(new LambdaElementBuilder(_matches, (def => builder)));
        }

        public void Modify(TagModifier modifier)
        {
            _factory.AddModifier(new LambdaElementModifier(_matches, (def => modifier)));
        }

        public void Modify(Action<HtmlTag> action)
        {
            Modify(((request, tag) => action(tag)));
        }

        public void BuildBy(TagBuilder builder)
        {
            registerBuilder(builder);
        }

        public void UseTextbox()
        {
            BuildBy(BuildTextbox);
        }

        public void AddClass(string className)
        {
            Modify(((r, tag) => tag.AddClass(className)));
        }

        public void Attr(string attName, object value)
        {
            Modify(((r, tag) => tag.Attr(attName, value)));
        }

        public static HtmlTag BuildTextbox(ElementRequest request)
        {
            return new TextboxTag().Attr("value", request.StringValue());
        }

        public static HtmlTag BuildCheckbox(ElementRequest request)
        {
            return new CheckboxTag((bool) request.RawValue);
        }
    }
}