using System;
using System.Linq;
using System.Web.Mvc;
using CC.Core.Core.DomainTools;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Config;
using MF.Web.Controllers;

namespace MF.Web.Areas.Billing.Controllers
{
    /// <summary>
    /// Lets an administrator run session completion on demand:
    /// POST /Billing/SessionCompletion/Run
    /// Production completes sessions with a scheduled Lambda; this exists so the
    /// same step can be triggered on QA, where nothing is scheduled.
    /// </summary>
    public class SessionCompletionController : MFController
    {
        private readonly ISessionCompletionService _sessionCompletionService;
        private readonly ISessionContext _sessionContext;
        private readonly IRepository _repository;

        public SessionCompletionController(ISessionCompletionService sessionCompletionService,
                                           ISessionContext sessionContext,
                                           IRepository repository)
        {
            _sessionCompletionService = sessionCompletionService;
            _sessionContext = sessionContext;
            _repository = repository;
        }

        [HttpPost]
        public CustomJsonResult Run()
        {
            var user = _repository.Find<User>(_sessionContext.GetUserId());
            if (user == null || !user.UserRoles.Any(x => x.Role.Name == UserType.Administrator.ToString()))
            {
                return new CustomJsonResult(new { Success = false, Message = "Only an administrator can run session completion." });
            }

            try
            {
                var completed = _sessionCompletionService.CompleteDueAppointments();
                return new CustomJsonResult(new
                {
                    Success = true,
                    Message = string.Format("{0} appointment(s) completed, {1} in arrears.",
                                            completed.Count, completed.Count(x => x.InArrears)),
                    Appointments = completed
                });
            }
            catch (Exception ex)
            {
                return new CustomJsonResult(new { Success = false, Message = ex.Message });
            }
        }
    }
}
