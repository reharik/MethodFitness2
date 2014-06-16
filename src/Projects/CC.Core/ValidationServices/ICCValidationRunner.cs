using System.Linq;
using CC.Core.ValidationServices;
using CC.DataValidation;

public interface ICCValidationRunner
{
    ValidationReport Validate<MODEL>(MODEL model) where MODEL : class;
}

public class CCValidationRunner : ICCValidationRunner
{
    private readonly IValidationRunner _validationRunner;

    public CCValidationRunner(IValidationRunner validationRunner)
    {
        _validationRunner = validationRunner;
    }

    public ValidationReport Validate<MODEL>(MODEL model) where MODEL : class
    {
        var errors = model != null ? _validationRunner.Validate(model) : new ErrorInfo[] { };
        var crudReport = new ValidationReport
        {
            Success = !errors.Any(),
        };
        crudReport.AddErrorInfos(errors);
        return crudReport;
    }
}