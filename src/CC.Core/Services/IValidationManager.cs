using System;
using System.Collections.Generic;
using System.Linq;
using CC.Core.Domain;
using CC.Core.DomainTools;
using xVal.ServerSide;

namespace CC.Core.Services
{
    public interface IValidationManager 
    {
        IEnumerable<ValidationReport> GetValidationReports();
        ValidationReport GetLastValidationReport();
        void RemoveValidationReport(ValidationReport validationReport);
        void AddValidationReport(ValidationReport validationReport);
        Notification Finish(string successMessage = "");
        bool HasFailed();
        Notification FinishWithAction(string successMessage = "");
    }

    public class ValidationManager : IValidationManager    {
        private readonly IRepository _repository;

        public ValidationManager(IRepository repository)
        {
            _repository = repository;
        }

        #region Collections
        private readonly IList<ValidationReport> _validationReports = new List<ValidationReport>();
        public IEnumerable<ValidationReport> GetValidationReports() { return _validationReports; }
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
            if (_validationReports.Any(crudReport => !crudReport.Success))
            {
                return true;
            }
            return false;
        }

        public Notification FinishWithAction(string successMessage = "")
        {
            if (successMessage.IsEmpty()) successMessage = CCCoreLocalizationKeys.SUCCESSFUL_SAVE.ToString();
            var notification = new Notification { Success = true };
            GetValidationReports().ForEachItem(x =>
            {
                if (!x.Success)
                {
                    notification.Success = false;
                    if (notification.Errors == null)
                        notification.Errors = x.GetErrorInfos().ToList();
                    else
                        x.GetErrorInfos().ForEachItem(notification.Errors.Add).ToList();
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
            if (successMessage.IsEmpty()) successMessage = CCCoreLocalizationKeys.SUCCESSFUL_SAVE.ToString();
            var notification = new Notification { Success = true };
            GetValidationReports().ForEachItem(x =>
            {
                if (!x.Success)
                {
                    notification.Success = false;
                    if (notification.Errors == null)
                        notification.Errors = x.GetErrorInfos().ToList();
                    else
                        x.GetErrorInfos().ForEachItem(notification.Errors.Add).ToList();
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