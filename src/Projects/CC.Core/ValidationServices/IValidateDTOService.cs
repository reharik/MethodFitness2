using System.Linq;
using CC.Utility;

namespace CC.Core.ValidationServices
{
    public interface IValidateDTOService
    {
        Continuation ValidateDTO<VIEWMODEL>(VIEWMODEL model, string successMessage = "") where VIEWMODEL : class;
    }

    public class ValidateDTOService : IValidateDTOService
    {
        private readonly ICCValidationRunner _xoValidationRunner;

        public ValidateDTOService(ICCValidationRunner xoValidationRunner)
        {
            _xoValidationRunner = xoValidationRunner;
        }

        public Continuation ValidateDTO<VIEWMODEL>(VIEWMODEL model, string successMessage = "") where VIEWMODEL : class
        {
            var report = _xoValidationRunner.Validate(model);
            if (successMessage.IsEmpty()) successMessage = "Yay, Success";
            var notification = new Continuation { Success = true };
            if (!report.Success)
            {
                notification.Success = false;
                notification.Errors = report.GetErrorInfos().ToList();
            }
            else
            {
                notification.Message = successMessage;
            }
            return notification;
        }
    }
}