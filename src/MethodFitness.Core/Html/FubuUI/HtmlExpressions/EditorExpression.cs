using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using CCUIHelpers.Tags;
using MethodFitness.Core.Html.FubuUI.Tags;
using MethodFitness.Security.Interfaces;
using MethodFitness.Core.Localization;
using HtmlTags;
using MethodFitness.Core.Services;
using StructureMap;

namespace MethodFitness.Core.Html.FubuUI.HtmlExpressions
{
    public class EditorExpression<VIEWMODEL> where VIEWMODEL : class
    {
        private readonly ITagGenerator<VIEWMODEL> _generator;
        private readonly Expression<Func<VIEWMODEL, object>> _expression;
        private HtmlTag _htmlRoot;
        private bool _inlineReverse;
        private IEnumerable<SelectListItem> _dropdownWithItems;
        private string _labelRootClass;
        private string _labelClass;
        private string _inputRootClass;
        private string _inputClass;
        private bool _hideRoot;
        private bool _hideInput;
        private bool _hideLabel;
        private string _labelId;
        private string _inputId;
        private string _rootId;
        private bool _dropdown;
        private bool _noClear;
        private string _labelDisplay;
        private string _radioButtonGroupName;
        private bool _radioButton;
        private bool _inline;
        private string _operation;
        private IAuthorizationService _authorizationService;
        private ISessionContext _sessionContext;
        private string _elType;
        private List<string> _rootClasses;
        private bool _readOnly;

        public EditorExpression(ITagGenerator<VIEWMODEL> generator, Expression<Func<VIEWMODEL, object>> expression)
        {
            _generator = generator;
            _expression = expression;
        }

        public override string ToString()
        {
            return ToHtmlTag() != null ? ToHtmlTag().ToString() : "";
        }

        public HtmlTag ToHtmlTag()
        {
            if (_operation.IsNotEmpty() && !checkAuthentication())
            {
                return null;
            }
            if (_inlineReverse)
            {
                return renderInlineReverse();
            }
            return renderStandard();
        }

        private bool checkAuthentication()
        {
            _authorizationService = ObjectFactory.Container.GetInstance<IAuthorizationService>();
            _sessionContext = ObjectFactory.Container.GetInstance<ISessionContext>();
            var user = _sessionContext.GetCurrentUser();
            return _authorizationService.IsAllowed(user, _operation);
        }

        private HtmlTag renderStandard()
        {
            _htmlRoot = new HtmlTag("div");
            _htmlRoot.AddClass(_noClear ? "editor_root_no_clear" : "editor_root");
            if (_rootId.IsNotEmpty()) _htmlRoot.Id(_rootId);
            if (_rootClasses != null && _rootClasses.Any()) _htmlRoot.AddClasses(_rootClasses);
            EditorLabelExpression<VIEWMODEL> labelBuilder = new EditorLabelExpression<VIEWMODEL>(_generator, _expression);
            IEditorInputExpression<VIEWMODEL> inputBuilder;
            if (_dropdown)
            {
                inputBuilder = new DropdownInputExpression<VIEWMODEL>(_generator, _expression, _dropdownWithItems);
            }
            else
            {
                inputBuilder = new EditorInputExpression<VIEWMODEL>(_generator, _expression);
            }
            addInternalCssClasses(labelBuilder, inputBuilder);
            hideElements(_htmlRoot, labelBuilder, inputBuilder);
            addIds(labelBuilder, inputBuilder);
            addCustomLabel(labelBuilder);
            HtmlTag input = inputBuilder.ToHtmlTag();
            HtmlTag label = labelBuilder.ToHtmlTag();
            _htmlRoot.Children.Add(label);
            _htmlRoot.Children.Add(input);
            addFlagToHtmlRoot(input.FirstChild());
            return _htmlRoot;
        }

        private void addFlagToHtmlRoot(HtmlTag input)
        {
            if (_elType.IsNotEmpty())
            {
                _htmlRoot.Attr("eltype", _elType);
                return;
            }
            if (input is TextboxTag)
            {
                if (input.HasClass("number"))
                {
                    _htmlRoot.Attr("eltype", "NumberTextbox");
                    return;
                }
                if (input.HasClass("datePicker"))
                {
                    _htmlRoot.Attr("eltype", "DateTextbox");
                    return;
                }
                if (input.HasClass("timePicker"))
                {
                    _htmlRoot.Attr("eltype", "TimeTextbox");
                    return;
                }
                _htmlRoot.Attr("eltype", "Textbox");
                return;
            }
            if (input is PasswordTag)
            {
                _htmlRoot.Attr("eltype", "Password");
                return;
            }
            if (input is SelectTag)
            {
                _htmlRoot.Attr("eltype", "Select");
                return;
            }
            if (input is CheckboxTag)
            {
                _htmlRoot.Attr("eltype", "Checkbox");
                return;
            }
            if (input.TagName() == "textarea")
            {
                _htmlRoot.Attr("eltype", "Textarea");
                return;
            }
            if (input.TagName() == "ul")
            {
                _htmlRoot.Attr("eltype", "PictureGallery");
                return;
            }
            if (input.HasClass("imageInputContainer"))
            {
                _htmlRoot.Attr("eltype", "FileSubmission");
                return;
            }

        }

        private void addCustomLabel(EditorLabelExpression<VIEWMODEL> label)
        {
            if (_labelDisplay.IsNotEmpty()) label.CustomLabel(_labelDisplay);
        }

        private void addIds(EditorLabelExpression<VIEWMODEL> label, IEditorInputExpression<VIEWMODEL> input)
        {
            if (_inputId.IsNotEmpty()) input.ElementId(_inputId);
            if (_labelId.IsNotEmpty()) label.ElementId(_labelId);
        }

        private HtmlTag renderInlineReverse()
        {
            _htmlRoot = new HtmlTag("div").AddClass("MF_editor_root");
            if (_rootId.IsNotEmpty()) _htmlRoot.Id(_rootId);
            if (_rootClasses != null && _rootClasses.Any()) _htmlRoot.AddClasses(_rootClasses);
            EditorLabelExpression<VIEWMODEL> labelBuilder = new EditorLabelExpression<VIEWMODEL>(_generator, _expression);
            EditorInputExpression<VIEWMODEL> inputBuilder = new EditorInputExpression<VIEWMODEL>(_generator, _expression);
            addInternalCssClasses(labelBuilder, inputBuilder);
            hideElements(_htmlRoot, labelBuilder, inputBuilder);
            addIds(labelBuilder, inputBuilder);
            addCustomLabel(labelBuilder);
            HtmlTag label = labelBuilder.LeadingColon().ToHtmlTag();
            HtmlTag input = inputBuilder.ToHtmlTag();
            _htmlRoot.Children.Add(input);
            _htmlRoot.Children.Add(label);
            return _htmlRoot;
        }

        private void hideElements(HtmlTag root, EditorLabelExpression<VIEWMODEL> label, IEditorInputExpression<VIEWMODEL> input)
        {
            if (_hideRoot) root.Hide();
            if (_hideLabel) label.Hide();
            if (_hideInput) input.Hide();
        }

        private void addInternalCssClasses(EditorLabelExpression<VIEWMODEL> labelBuilder, IEditorInputExpression<VIEWMODEL> inputBuilder)
        {
            if (_labelRootClass.IsNotEmpty()) labelBuilder.AddClassToLabelRoot(_labelRootClass);
            if (_labelClass.IsNotEmpty()) labelBuilder.AddClassToLabel(_labelClass);
            if (_inputRootClass.IsNotEmpty()) inputBuilder.AddClassToInputRoot(_inputRootClass);
            if (_inputClass.IsNotEmpty()) inputBuilder.AddClassToInput(_inputClass);
        }

        #region Extensions

        public EditorExpression<VIEWMODEL> RadioButton()
        {
            _inlineReverse = true;
            _radioButton = true;
            return this;
        }
        public EditorExpression<VIEWMODEL> RadioButton(string groupName)
        {
            _inlineReverse = true;
            _radioButtonGroupName = groupName;
            _radioButton = true;
            return this;
        }

        public EditorExpression<VIEWMODEL> LabelDisplay(StringToken display)
        {
            _labelDisplay = display.ToString();
            return this;
        }

        public EditorExpression<VIEWMODEL> LabelDisplay(string display)
        {
            _labelDisplay = display;
            return this;
        }

        public EditorExpression<VIEWMODEL> NoClear()
        {
            _noClear = true;
            return this;
        }

        public EditorExpression<VIEWMODEL> InLine()
        {
            _inline = true;
            return this;
        }

        public EditorExpression<VIEWMODEL> InlineReverse()
        {
            _inlineReverse = true;
            return this;
        }

        public EditorExpression<VIEWMODEL> FillWith(IEnumerable<SelectListItem> enumerable)
        {
            _dropdown = true;
            _dropdownWithItems = enumerable;
            return this;
        }
        public EditorExpression<VIEWMODEL> AddClassToRoot(string cssClass)
        {
            if (_rootClasses == null)
            {
                _rootClasses = new List<string>();
            }
            if (cssClass.Contains(" "))
            {
                cssClass.Split(' ').ForEachItem(_rootClasses.Add);
            }
            else
            {
                _rootClasses.Add(cssClass);
            }
            return this;
        }
       
        public EditorExpression<VIEWMODEL> AddClassToLabelRoot(string cssClass)
        {
            _labelRootClass = cssClass;
            return this;
        }

        public EditorExpression<VIEWMODEL> AddClassToLabel(string cssClass)
        {
            _labelClass = cssClass;
            return this;
        }

        public EditorExpression<VIEWMODEL> AddClassToInputRoot(string cssClass)
        {
            _inputRootClass = cssClass;
            return this;
        }

        public EditorExpression<VIEWMODEL> AddClassToInput(string cssClass)
        {
            _inputClass = cssClass;
            return this;
        }

        public EditorExpression<VIEWMODEL> HideRoot()
        {
            _hideRoot = true;
            return this;
        }

        public EditorExpression<VIEWMODEL> HideLabel()
        {
            _hideLabel = true;
            return this;
        }

        public EditorExpression<VIEWMODEL> HideInput()
        {
            _hideInput = true;
            return this;
        }

        public EditorExpression<VIEWMODEL> RootId(string id)
        {
            _rootId = id;
            return this;
        }

        public EditorExpression<VIEWMODEL> InputId(string id)
        {
            _inputId = id;
            return this;
        }

        public EditorExpression<VIEWMODEL> labelId(string id)
        {
            _labelId = id;
            return this;
        }

        public EditorExpression<VIEWMODEL> OperationName(string operation)
        {
            _operation = operation;
            return this;
        }

        public EditorExpression<VIEWMODEL> ElType(string elType)
        {
            _elType = elType;
            return this;
        }

        public EditorExpression<VIEWMODEL> ReadOnly()
        {
            _readOnly = true;
            return this;
        } 
        #endregion

    }
}