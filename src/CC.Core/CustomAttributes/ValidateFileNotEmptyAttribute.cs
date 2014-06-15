using System;
using System.Text.RegularExpressions;
using Castle.Components.Validator;

namespace CC.Core.CustomAttributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Parameter,
        AllowMultiple = true)]
    public class ValidateFileNotEmptyAttribute : AbstractValidationAttribute
    {
        private IValidator validator;

        public ValidateFileNotEmptyAttribute()
        {
            validator = new UrlValidator();
        }

        public ValidateFileNotEmptyAttribute(string errorMessage)
            : base(errorMessage)
        {
            validator = new UrlValidator();
        }

        public override IValidator Build()
        {
            ConfigureValidatorMessage(validator);

            return validator;
        }
    }

    [Serializable()]
    public class FileNotEmptyValidator : AbstractValidator
    {
        private string defaultErrorMessage = CCCoreLocalizationKeys.FILE_IS_REQUIRED.ToString();

        public override bool IsValid(object instance, object fieldValue)
        {
            return fieldValue != null;
        }

        public override bool SupportsBrowserValidation
        {
            get { return true; }
        }

        protected override string BuildErrorMessage()
        {
            return string.Format(defaultErrorMessage,"Field");
        }
    }
}