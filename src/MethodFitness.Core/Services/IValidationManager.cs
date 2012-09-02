using System;
using System.Collections.Generic;
using System.Linq;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using xVal.ServerSide;

namespace MethodFitness.Core.Services
{
    public interface IValidationManager<ENTITY> where ENTITY:Entity
    {
        IEnumerable<ValidationReport<ENTITY>> GetValidationReports();
        ValidationReport<ENTITY> GetLastValidationReport();
        void RemoveValidationReport(ValidationReport<ENTITY> validationReport);
        void AddValidationReport(ValidationReport<ENTITY> validationReport);
        Notification Finish(string successMessage = "");
        bool HasFailed();
        Notification FinishWithAction(string successMessage = "");
    }

    public class ValidationManager<ENTITY> : IValidationManager<ENTITY> where ENTITY : Entity
    {
        private readonly IRepository _repository;

        public ValidationManager(IRepository repository)
        {
            _repository = repository;
        }

        #region Collections
        private readonly IList<ValidationReport<ENTITY>> _validationReports = new List<ValidationReport<ENTITY>>();
        public IEnumerable<ValidationReport<ENTITY>> GetValidationReports() { return _validationReports; }
        public ValidationReport<ENTITY> GetLastValidationReport()
        {
            return _validationReports.Last();
        }
        public void RemoveValidationReport(ValidationReport<ENTITY> validationReport)
        {
            _validationReports.Remove(validationReport);
        }
        public void AddValidationReport(ValidationReport<ENTITY> validationReport)
        {
            _validationReports.Add(validationReport);
        }
        #endregion

        public bool HasFailed()
        {
            if (_validationReports.Any(crudReport => !crudReport.Success))
            {
                return true;
            }
            return false;
        }

        public Notification FinishWithAction(string successMessage = "")
        {
            if (successMessage.IsEmpty()) successMessage = CoreLocalizationKeys.SUCCESSFUL_SAVE.ToString();
            var notification = new Notification { Success = true };
            GetValidationReports().ForEachItem(x =>
            {
                if (!x.Success)
                {
                    notification.Success = false;
                    if (notification.Errors == null)
                        notification.Errors = x.GetErrorInfos().ToList();
                    else
                        BasicExtentions.ForEachItem(x.GetErrorInfos(), notification.Errors.Add).ToList();
                }else
                {
                    x.SuccessAction(x.entity);
                }
            });
            if (notification.Success)
            {
                _repository.Commit();
                notification.Message = successMessage;
            }
            else
                _repository.Rollback();
            return notification;
        }

        public Notification Finish(string successMessage ="")
        {
            if (successMessage.IsEmpty()) successMessage = CoreLocalizationKeys.SUCCESSFUL_SAVE.ToString();
            var notification = new Notification { Success = true };
            GetValidationReports().ForEachItem(x =>
            {
                if (!x.Success)
                {
                    notification.Success = false;
                    if (notification.Errors == null)
                        notification.Errors = x.GetErrorInfos().ToList();
                    else
                        BasicExtentions.ForEachItem(x.GetErrorInfos(), notification.Errors.Add).ToList();
                }
            });
            if (notification.Success)
            {
                _repository.Commit();
                notification.Message = successMessage;
            }
            else
                _repository.Rollback();
            return notification;
        }
    }

    public class ValidationReport<ENTITY> where ENTITY:Entity
    {
        public ENTITY entity { get; set; }
        public Action<ENTITY> SuccessAction { get; set; }
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