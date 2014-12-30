using System;
using System.Collections.Generic;
using System.Linq;
using CC.Core.Core.Domain;
using CC.Core.Core.DomainTools;
using CC.Core.DataValidation;
using CC.Core.Utilities;

namespace CC.Core.Core.ValidationServices
{
    public interface IValidationManager
    {
        IEnumerable<ValidationReport> GetValidationReports();
        ValidationReport GetLastValidationReport();
        void RemoveValidationReport(ValidationReport validationReport);
        void AddValidationReport(ValidationReport validationReport);
        Continuation Finish(string successMessage = "");
        bool HasFailed();
        Continuation FinishWithAction(string successMessage = "");
    }

    public class ValidationManager : IValidationManager
    {
        private readonly IRepository _repository;

        public ValidationManager(IRepository repository)
        {
            _repository = repository;
        }

        #region Collections

        private readonly IList<ValidationReport> _validationReports = new List<ValidationReport>();

        public IEnumerable<ValidationReport> GetValidationReports()
        {
            return _validationReports;
        }

        public ValidationReport GetLastValidationReport()
        {
            return _validationReports.Last();
        }

        public void RemoveValidationReport(ValidationReport validationReport)
        {
            _validationReports.Remove(validationReport);
        }

        public void AddValidationReport(ValidationReport validationReport)
        {
            _validationReports.Add(validationReport);
        }

        #endregion

        public bool HasFailed()
        {
            return _validationReports.Any(crudReport => !crudReport.Success);
        }

        public Continuation Finish(string successMessage = "")
        {
            if (successMessage.IsEmpty()) successMessage = CCCoreLocalizationKeys.SUCCESSFUL_SAVE.ToString();
            var continuation = new Continuation {Success = true};
            GetValidationReports().ForEachItem(x =>
                {
                    if (!x.Success)
                    {
                        continuation.Success = false;
                        if (continuation.Errors == null)
                        {
                            continuation.Errors = Enumerable.ToList<ErrorInfo>(x.GetErrorInfos());
                        }
                        else
                        {
                            x.GetErrorInfos().ForEachItem(continuation.Errors.Add).ToList();
                        }
                    }
                });

            if (continuation.Success)
            {
                _repository.Commit();
                continuation.Message = successMessage;
            }
            else
            {
                _repository.Rollback();
            }

            return continuation;
        }

        public Continuation FinishWithAction(string successMessage = "")
        {
            if (successMessage.IsEmpty()) successMessage = CCCoreLocalizationKeys.SUCCESSFUL_SAVE.ToString();
            var continuation = new Continuation { Success = true };
            GetValidationReports().ForEachItem(x =>
                {
                    if (!x.Success)
                    {
                        continuation.Success = false;
                        if (continuation.Errors == null)
                            continuation.Errors = Enumerable.ToList<ErrorInfo>(x.GetErrorInfos());
                        else
                            x.GetErrorInfos().ForEachItem(continuation.Errors.Add).ToList();
                    }
                    else
                    {
                        x.SuccessAction(x.entity);
                    }
                });
            if (continuation.Success)
            {
                _repository.Commit();
                continuation.Message = successMessage;
            }
            else
                _repository.Rollback();
            return continuation;
        }
    }

    public class ValidationReport
    {
        public Entity entity { get; set; }
        public Action<Entity> SuccessAction { get; set; }
        public bool Success { get; set; }
        #region Collections
        private readonly IList<ErrorInfo> _errorInfos = new List<ErrorInfo>();
        public IEnumerable<ErrorInfo> GetErrorInfos() { return _errorInfos; }
        public void RemoveErrorInfo(ErrorInfo errorInfo)
        {
            _errorInfos.Remove(errorInfo);
        }
        public void AddErrorInfo(ErrorInfo errorInfo)
        {
            _errorInfos.Add(errorInfo);
        }
        public void AddErrorInfos(IEnumerable<ErrorInfo> errors)
        {
            _errorInfos.AddMany(errors.ToArray());
        }
        #endregion
    }
}