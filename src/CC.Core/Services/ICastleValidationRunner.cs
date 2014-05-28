using System;
using System.Collections.Generic;
using System.Linq;
using CC.Core.Domain;
using Castle.Components.Validator;
using xVal.ServerSide;

namespace CC.Core.Services
{
    public interface ICastleValidationRunner
    {
        IEnumerable<ErrorInfo> GetErrors<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject;
        ValidationReport Validate<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject;
    }

    public class DummyCastleValidationRunnerSuccess : ICastleValidationRunner
    {
        #region Implementation of ICastleValidationRunner

        public IEnumerable<ErrorInfo> GetErrors<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            throw new NotImplementedException();
        }

        public ValidationReport Validate<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            var crudReport = new ValidationReport { Success = true };
            return crudReport;
        }

        #endregion
    }

    public class DummyCastleValidationRunnerFail : ICastleValidationRunner
    {
        #region Implementation of ICastleValidationRunner

        public IEnumerable<ErrorInfo> GetErrors<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            throw new NotImplementedException();
        }

        public ValidationReport Validate<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            var crudReport = new ValidationReport { Success = false };
            crudReport.AddErrorInfo(new ErrorInfo("test", "test error"));
            return crudReport;
        }
        #endregion
    }

    public class CastleValidationRunner : ICastleValidationRunner
    {
        private static readonly CachedValidationRegistry registry = new CachedValidationRegistry();

        public IEnumerable<ErrorInfo> GetErrors<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            var result = new List<ErrorInfo>();
            var runner = new ValidatorRunner(registry);

            if (entity != null && !runner.IsValid(entity))
            {
                var errorSummary = runner.GetErrorSummary(entity);
                var errorInfos = errorSummary.InvalidProperties.SelectMany(
                    errorSummary.GetErrorsForProperty,
                    (prop, err) => new ErrorInfo(prop, err));
                result.AddRange(errorInfos);
            }
            return result;
        }

        public ValidationReport Validate<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            var crudReport = new ValidationReport();
            var runner = new ValidatorRunner(registry);
            if (runner.IsValid(entity))
            {
                crudReport.Success = true;
            }
            else
            {
                crudReport.AddErrorInfos(GetErrors(entity));
            }
            return crudReport;
        }
    }
}