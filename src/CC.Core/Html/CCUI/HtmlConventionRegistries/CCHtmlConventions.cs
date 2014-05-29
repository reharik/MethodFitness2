using System;
using System.Linq;
using CC.Core.Domain;
using CC.Core.Html.CCUI.Builders;
using CC.Core.Html.CCUI.Builders;
using CC.Core.Html.CCUI.Tags;
using CC.Core.Utilities;
using CC.UI.Helpers;
using CC.UI.Helpers.Configuration;
using CC.UI.Helpers.Tags;
using HtmlTags;

namespace CC.Core.Html.CCUI.HtmlConventionRegistries
{
    public class CCHtmlConventions : HtmlConventionRegistry
    {
        public CCHtmlConventions()
        {
            numbers();
            EditorsChain();
            DisplaysChain();
            LabelsChain();
            validationAttributes();
        }

        public virtual void LabelsChain()
        {
            Labels.Always.BuildBy(
                req =>
                new HtmlTag("label").Attr("for", req.Accessor.Name).Text(req.Accessor.FieldName.ToSeperateWordsFromPascalCase()));
        }

        public virtual void DisplaysChain()
        {
            Displays.Builder<ImageBuilder>();
            Displays.Builder<EmailDisplayBuilder>();
            Displays.Builder<ListDisplayBuilder>();
            Displays.Builder<LinkDisplayBuilder>();
            Displays.Builder<ImageFileDisplayBuilder>();
            Displays.Builder<DateFormatter>();
            Displays.Builder<TimeFormatter>();
            Displays.If(x => x.Accessor.PropertyType == typeof (DateTime) || x.Accessor.PropertyType == typeof (DateTime?))
                .BuildBy(
                    req =>
                    req.RawValue != null
                        ? new HtmlTag("span").Text(DateTime.Parse(req.RawValue.ToString()).ToLongDateString())
                        : new HtmlTag("span"));
            Displays.Always.BuildBy(req => new HtmlTag("span").Text(req.StringValue()));
        }

        public virtual void EditorsChain()
        {
            Editors.Builder<SelectFromEnumerationBuilder>();
            Editors.Builder<SelectFromIEnumerableBuilder>();
            Editors.Builder<GroupSelectedBuilder>();
            Editors.Builder<TextAreaBuilder>();
            Editors.Builder<DatePickerBuilder>();
            Editors.Builder<TimePickerBuilder>();
            Editors.Builder<CheckboxBuilder>();
            Editors.If(x => x.Accessor.Name.ToLowerInvariant().Contains("password")).BuildBy(
                r => new PasswordTag().Attr("value", r.RawValue));
            Editors.Always.BuildBy(TagActionExpression.BuildTextbox);
            Editors.Always.Modify(AddElementName);
        }

        public static void AddElementName(ElementRequest request, HtmlTag tag)
        {
            if (tag.IsInputElement())
            {
                var name = request.Accessor.Name;
                if (request.Accessor is PropertyChain)
                {
                    name = ((PropertyChain)(request.Accessor)).PropertyNames.Aggregate((current, next) => current + "." + next);
                    if (new InheritsFromDomainEntity().execute(request.Accessor.PropertyType))
                        name += ".EntityId";
                }
                //var name = request.Accessor.Name.Substring(0, request.Accessor.Name.IndexOf(request.Accessor.FieldName)) + "." + request.Accessor.FieldName; 
                //tag.Attr("name", name);
                tag.Attr("name", name);
            }
        }
        // I understand this is a retarded way to do it but I can't figure it rigt now
        private class InheritsFromDomainEntity
        {
            private bool check(Type type)
            {
                if (type == typeof(Entity) || type.BaseType == typeof(Entity))
                    return true;
                return type.BaseType != null && check(type.BaseType);
            }

            public bool execute(Type type)
            {
                var result = false;
                if (type.BaseType != null)
                    result = check(type.BaseType);
                return result;
            }
        }

        private void numbers()
        {
            Editors.IfPropertyIs<Int32>().Attr("max", Int32.MaxValue);
            Editors.IfPropertyIs<Int16>().Attr("max", Int16.MaxValue);
            //Editoin.IfPropertyIs<Int64>().Attr("max", Int64.MaxValue);
            Editors.IfPropertyTypeIs(IsIntegerBased).AddClass("integer");
            Editors.IfPropertyTypeIs(IsFloatingPoint).AddClass("number");
            Editors.IfPropertyTypeIs(IsIntegerBased).Attr("mask", "wholeNumber");
        }

        private void validationAttributes()
        {
            Editors.Modifier<RequiredModifier>();
            Editors.Modifier<PasswordConfirmModifier>();
            Editors.Modifier<EmailModifier>();
            Editors.Modifier<NumberModifier>();
            Editors.Modifier<UrlModifier>();
            Editors.Modifier<DateModifier>();
            Editors.Modifier<RangeModifier>();
        }

        public static bool IsIntegerBased(Type type)
        {
            return type == typeof(int) || type == typeof(long) || type == typeof(short);
        }

        public static bool IsFloatingPoint(Type type)
        {
            return type == typeof(decimal) || type == typeof(float) || type == typeof(double);
        }
    }
}