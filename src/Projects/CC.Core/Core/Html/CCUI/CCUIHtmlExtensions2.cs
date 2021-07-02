using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CC.Core.Core.Html.CCUI.HtmlExpressions;
using CC.Core.HtmlTags;
using CC.Core.Reflection;
using CC.Core.UI.Helpers.Tags;
using CC.Core.Utilities;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CC.Core.Core.Html.CCUI
{
    public class CCUIHtmlExtensions2 : HtmlHelper
    {
        private readonly ITagGenerator<T> _generator;

        public CCUIHtmlExtensions2(ITagGenerator<T> generator) {
            _generator = generator;

        }
        private ITagGenerator<T> GetGenerator(IHtmlHelper<T> helper, Expression<Func<T, object>> expression)
        {
            //TagGenerator<T> generator = DependencyResolver.Current.GetService<ITagGenerator<T>>() as TagGenerator<T>;
            _generator.Model = helper.ViewData.Model;
            if (helper.ViewData.TemplateInfo.HtmlFieldPrefix.IsNotEmpty())
            {
                _generator.ElementPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix + ".";
            }
            else
            {
                Accessor accessor = expression.ToAccessor();
                if (!accessor.OwnerType.Name.ToLowerInvariant().Contains("viewmodel"))
                {
                    _generator.ElementPrefix = accessor.OwnerType.Name + ".";
                }
            }
            return _generator;
        }

        public HtmlTag InputCC(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression)
        {
            ITagGenerator<T> generator = GetGenerator(helper, expression);
            return generator.InputFor(expression);
        }
        
        public HtmlTag LabelCC<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            ITagGenerator<T> generator = GetGenerator<T>(helper, expression);
            HtmlTag tag = generator.LabelFor(expression);
            return tag;
        }

        public  HtmlTag DisplayCC<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            ITagGenerator<T> generator = GetGenerator<T>(helper, expression);
            return generator.DisplayFor(expression);
        }

        public  EditorExpression<T> EditorInlineReverse<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            ITagGenerator<T> generator = GetGenerator(helper, expression);
            return new EditorExpression<T>(generator, expression).InlineReverse();
        }

        public  EditorExpression<T> SubmissionFor<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            ITagGenerator<T> generator = GetGenerator(helper, expression);
            return new EditorExpression<T>(generator, expression);
        }

        public  EditorExpression<T> DropdownSubmissionFor<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression, IEnumerable<SelectListItem> fillWith) where T : class
        {
            ITagGenerator<T> generator = GetGenerator(helper, expression);
            return new EditorExpression<T>(generator, expression).FillWith(fillWith);
        }

        public  ViewExpression<T> ViewFor<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            ITagGenerator<T> generator = GetGenerator(helper, expression);
            return new ViewExpression<T>(generator, expression);
        }

        public  ViewDisplayExpression<T> ViewDisplayFor<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            ITagGenerator<T> generator = GetGenerator(helper, expression);
            return new ViewDisplayExpression<T>(generator, expression);
        }

        public  ViewDisplayDataRangeExpression<T> ViewDisplayDateRangeFor<T>(this HtmlHelper<T> helper, 
                                                                            Expression<Func<T, object>> expressionFrom,
                                                                            Expression<Func<T, object>> expressionTo,
                                                                            Expression<Func<T, object>> expressionToPresent = null) where T : class
        {
            ITagGenerator<T> generator = GetGenerator(helper, expressionFrom);
            return new ViewDisplayDataRangeExpression<T>(generator, expressionFrom, expressionTo, expressionToPresent);
        }


        public  ViewDisplayCityStateExpression<T> ViewDisplayCityStateFor<T>(this HtmlHelper<T> helper,
                                                                           Expression<Func<T, object>> expressionCity,
                                                                           Expression<Func<T, object>> expressionState,
                                                                           Expression<Func<T, object>> expressionZip) where T : class
        {
            ITagGenerator<T> generator = GetGenerator(helper, expressionCity);
            return new ViewDisplayCityStateExpression<T>(generator, expressionCity, expressionState, expressionZip);
        }

        public  ViewDisplayAddressExpression<T> ViewDisplayAddressFor<T>(this HtmlHelper<T> helper,
              Expression<Func<T, object>> address) where T : class
        {
            ITagGenerator<T> generator = GetGenerator(helper, address);
            return new ViewDisplayAddressExpression<T>(generator, address);
        }



        //public  string ElementNameFor<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        //{
        //    var convention = ObjectFactory.Container.GetInstance<IElementNamingConvention>();
        //    return convention.GetName(typeof(T), expression.ToAccessor());
        //}

    }

    
}