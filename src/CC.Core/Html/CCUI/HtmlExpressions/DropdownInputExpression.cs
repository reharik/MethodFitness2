using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using CC.Core.Utilities;
using CC.UI.Helpers.Configuration;
using CC.UI.Helpers.Tags;
using HtmlTags;

namespace CC.Core.Html.CCUI.HtmlExpressions
{
    public class DropdownInputExpression<VIEWMODEL> : IEditorInputExpression<VIEWMODEL> where VIEWMODEL : class
    {
        private readonly ITagGenerator<VIEWMODEL> _generator;
        private readonly Expression<Func<VIEWMODEL, object>> _expression;
        private readonly IEnumerable<SelectListItem> _items;
        private HtmlTag _htmlRoot;
        private string _inputRootClass;
        private string _inputClass;
        private bool _hide;
        private string _elementId;
        private string _labelDisplay;
        private bool _readOnly;

        public DropdownInputExpression(ITagGenerator<VIEWMODEL> generator, Expression<Func<VIEWMODEL, object>> expression, IEnumerable<SelectListItem> items)
        {
            _generator = generator;
            _expression = expression;
            _items = items;
        }

        public HtmlTag ToHtmlTag()
        {
            _htmlRoot = new HtmlTag("div").AddClass("editor_input");
            if (_hide) _htmlRoot.Hide();
            ElementRequest request = _generator.GetRequest(_expression);

            Action<SelectTag> action = x =>
                                           {
                                               var value = request.RawValue;
                                               if (_items!=null)
                                               {
                                                   foreach (SelectListItem option in _items)
                                                   {
                                                       x.Option(option.Text, option.Value);
                                                   }
                                                   if (value != null && value.ToString().IsNotEmpty())
                                                   {
                                                       x.SelectByValue(value.ToString());
                                                   }
                                                   else
                                                   {
                                                       SelectListItem defaultOption =
                                                           _items.FirstOrDefault(o => o.Selected);
                                                       if (defaultOption != null)
                                                       {
                                                           x.SelectByValue(defaultOption.Value);
                                                       }
                                                   }
                                               }
                                           };
            SelectTag tag = new SelectTag(action);
            string name = string.Empty;
            request.Accessor.PropertyNames.ForEachItem(x => name += x + ".");
            name = name.Substring(0, name.Length-1);
            tag.Attr("name", name);
            addInternalCssClasses(_htmlRoot, tag);
            if (_elementId.IsNotEmpty()) tag.Id(_elementId);
            _htmlRoot.Append(tag);
            return _htmlRoot;
        }

        private void addInternalCssClasses(HtmlTag root, HtmlTag input)
        {
            if (input.GetValidationHelpers().Any())
            {
                var origional = ReflectionHelper.GetProperty(_expression).Name;
                input.GetValidationHelpers().ForEachItem(x => x.ErrorMessage = x.ErrorMessage.Replace(origional, _labelDisplay));
            }
            if (_inputRootClass.IsNotEmpty()) root.AddClass(_inputRootClass);
            if (_inputClass.IsNotEmpty()) input.AddClass(_inputClass);
        }

        public IEditorInputExpression<VIEWMODEL> AddClassToInputRoot(string cssClass)
        {
            _inputRootClass = cssClass;
            return this;
        }

        public IEditorInputExpression<VIEWMODEL> AddClassToInput(string cssClass)
        {
            _inputClass = cssClass;
            return this;
        }

        public IEditorInputExpression<VIEWMODEL> Hide()
        {
            _hide = true;
            return this;
        }

        public IEditorInputExpression<VIEWMODEL> ElementId(string id)
        {
            _elementId = id;
            return this;
        }

        public IEditorInputExpression<VIEWMODEL> CustomLabel(string labelDisplay)
        {
            _labelDisplay = labelDisplay;
            return this;
        }

        public IEditorInputExpression<VIEWMODEL> ReadOnly()
        {
            _readOnly = true;
            return this;
        }
    }
}