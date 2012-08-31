using System;
using System.Linq;
using KnowYourTurf.Core.Domain;
using KnowYourTurf.Core.Html.FubuUI.Builders;
using KnowYourTurf.Core.Html.FubuUI.Tags;
using FubuMVC.UI;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using HtmlTags;
using KnowYourTurf.Core;

namespace KnowYourTurf.Core.Html.FubuUI.HtmlConventionRegistries
{
    public class KnowYourTurfHtmlConventions2 : HtmlConventionRegistry
    {
        public KnowYourTurfHtmlConventions2()
        {
            Editors.Builder<SelectFromEnumerationBuilder2>();
            Editors.Builder<SelectFromIEnumerableBuilder2>();
            Editors.Builder<GroupSelectedBuilder2>();
            Editors.Builder<TextAreaBuilder2>();
            Editors.Builder<DatePickerBuilder2>();
            Editors.Builder<TimePickerBuilder2>();
            Editors.Builder<CheckboxBuilder2>();
            Editors.Builder<PasswordBuilder2>();
            Editors.Builder<MultiSelectBuilder2>();
            Editors.Builder<PictureGallery>();
            Editors.Builder<FileUploader>();
            // default builder
            Editors.Builder<TextboxBuilder2>();
            Editors.Always.Modify(AddElementName);
            ///
            Displays.Builder<ImageBuilder2>();
            Displays.Builder<EmailDisplayBuilder2>();
            Displays.Builder<ListDisplayBuilder>();
            Displays.Builder<DateDisplayBuilder2>();
            Displays.Builder<TimeDisplayBuilder2>();
            Displays.Builder<ImageFileDisplayBuilder>();
            Displays.Always.BuildBy(req =>
                                        {
                                            var placeHolder = new HtmlTag("span").Text(" ");
                                                placeHolder.Children.Add(new HtmlTag("span").Attr("data-bind", "text:" + KnowYourTurfHtmlConventions.DeriveElementName(req)));
                                            return placeHolder;
                                        });
            Labels.Always.BuildBy(req =>
                                      {
                                          var htmlTag = new HtmlTag("label").Attr("for", req.Accessor.Name);
                                          var display = req.Accessor.FieldName;
                                          htmlTag.Text(display.ToSeperateWordsFromPascalCase());
                                          return htmlTag;
                                      });
            numbers();
            validationAttributes();

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
            if (request.Accessor is FubuMVC.Core.Util.PropertyChain)
            {
                name = ((FubuMVC.Core.Util.PropertyChain)(request.Accessor)).Names.Aggregate((current, next) => current + "." + next);
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
            //Editors.IfPropertyIs<Int64>().Attr("max", Int64.MaxValue);
            Editors.IfPropertyTypeIs(IsIntegerBased).Modify(x=>{if(x.TagName()==new TextboxTag().TagName()) x.AddClass("integer");});
            Editors.IfPropertyTypeIs(IsFloatingPoint).Modify(x=>{if(x.TagName()==new TextboxTag().TagName()) x.AddClass("number");});
            Editors.IfPropertyTypeIs(IsIntegerBased).Modify(x => { if (x.TagName() == new TextboxTag().TagName()) x.Attr("mask", "wholeNumber"); });
        }

        private void validationAttributes()
        {
            Editors.Modifier<RequiredModifier>();
            Editors.Modifier<NumberModifier>();
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