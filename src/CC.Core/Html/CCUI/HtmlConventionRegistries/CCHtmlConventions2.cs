using System;
using System.Linq;
using CC.Core.Html.CCUI.Builders;
using CC.Core.Html.CCUI.Builders;
using CC.Core.Utilities;
using CC.UI.Helpers;
using CC.UI.Helpers.Configuration;
using CC.UI.Helpers.Tags;
using HtmlTags;

namespace CC.Core.Html.CCUI.HtmlConventionRegistries
{
    public class CCHtmlConventions2 : HtmlConventionRegistry
    {
        public CCHtmlConventions2()
        {
            EditorsChain();
            DisplaysChain();
            LabelsChain();
            
            numbers();
            validationAttributes();

        }

        public virtual void LabelsChain()
        {
           Labels.Always.BuildBy(req =>
                                      {
                                          var htmlTag = new HtmlTag("label").Attr("for", req.Accessor.Name);
                                          var display = req.Accessor.FieldName;
                                          htmlTag.Text(display.ToSeperateWordsFromPascalCase());
                                          return htmlTag;
                                      });
        }

        public virtual void DisplaysChain()
        {
            Displays.Builder<ImageBuilder2>();
            Displays.Builder<EmailDisplayBuilder2>();
            Displays.Builder<ListDisplayBuilder>();
            Displays.Builder<DateDisplayBuilder2>();
            Displays.Builder<TimeDisplayBuilder2>();
            Displays.Builder<ImageFileDisplayBuilder>();
            Displays.Always.BuildBy(req => new HtmlTag("span").Attr("data-bind", "text:" + DeriveElementName(req)));
        }

        public virtual void EditorsChain()
        {
            Editors.Builder<SelectFromEnumerationBuilder2>();
            Editors.Builder<SelectFromIEnumerableBuilder2>();
            Editors.Builder<GroupSelectedBuilder2>();
            Editors.Builder<TextAreaBuilder2>();
            Editors.Builder<DatePickerBuilder2>();
//            EditorsChain.Builder<TimePickerBuilder2>();
            Editors.Builder<CheckboxBuilder2>();
            Editors.Builder<PasswordBuilder2>();
            Editors.Builder<MultiSelectBuilder2>();
//            EditorsChain.Builder<PictureGallery>();
            Editors.Builder<FileUploader>();
            // default builder
            Editors.Builder<TextboxBuilder2>();
            Editors.Always.Modify(AddElementName);
        }

        public static void AddElementName(ElementRequest request, HtmlTag tag)
        {
            if (tag.IsInputElement())
            {
                tag.Attr("name", DeriveElementName(request));
            }
        }

        public static string DeriveElementName(ElementRequest request)
        {
            var name = request.Accessor.Name;
            if (request.Accessor is PropertyChain)
            {
                name = ((PropertyChain)(request.Accessor)).PropertyNames.Aggregate((current, next) => current + "." + next);
                var isDomainEntity = false;
                var de = request.Accessor.PropertyType.BaseType;
                while (de.Name != "Object")
                {
                    if (de.Name == "DomainEntity") isDomainEntity = true;
                    de = de.BaseType;
                }
                if (isDomainEntity) name += ".EntityId";

            }
            return name;
        }

        private void numbers()
        {
            Editors.IfPropertyIs<Int32>().Modify(x=>{if(x.TagName()==new TextboxTag().TagName()) x.Attr("max", Int32.MaxValue);});
            Editors.IfPropertyIs<Int16>().Modify(x=>{if(x.TagName()==new TextboxTag().TagName()) x.Attr("max", Int16.MaxValue);});
            //EditorsChain.IfPropertyIs<Int64>().Attr("max", Int64.MaxValue);
            Editors.IfPropertyTypeIs(IsIntegerBased).Modify(x=>{if(x.TagName()==new TextboxTag().TagName()) x.AddClass("integer");});
            Editors.IfPropertyTypeIs(IsFloatingPoint).Modify(x=>{if(x.TagName()==new TextboxTag().TagName()) x.AddClass("number");});
            Editors.IfPropertyTypeIs(IsIntegerBased).Modify(x => { if (x.TagName() == new TextboxTag().TagName()) x.Attr("mask", "wholeNumber"); });
        }

        private void validationAttributes()
        {
            Editors.Modifier<RequiredModifier>();
            Editors.Modifier<NumberModifier>();
            Editors.Modifier<FileRequiredModifier>();
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