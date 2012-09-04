using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.Validator;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using xVal.ServerSide;

namespace MethodFitness.Core.Services
{
    public interface ICastleValidationRunner
    {
        IEnumerable<ErrorInfo> GetErrors<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject;
        ValidationReport<ENTITY> Validate<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject;
    }

    public class DummyCastleValidationRunnerSuccess : ICastleValidationRunner
    {
        #region Implementation of ICastleValidationRunner

        public IEnumerable<ErrorInfo> GetErrors<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            throw new NotImplementedException();
        }

        public ValidationReport<ENTITY> Validate<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            var crudReport = new ValidationReport<ENTITY> { Success = true };
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

        public ValidationReport<ENTITY> Validate<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            var crudReport = new ValidationReport<ENTITY> { Success = false };
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

        public ValidationReport<ENTITY> Validate<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            var crudReport = new ValidationReport<ENTITY>();
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