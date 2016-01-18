using System;
using System.Linq;
using CC.Core.Core.DomainTools;
using CC.Core.Core.ValidationServices;
using CC.Core.Utilities;
using MF.Core.Domain;

namespace MF.Core.Services
{
    public interface ISessionManager
    {
//        void GatherAppointmentsDue();
        void CompleteAppointments();
    }

    public class SessionManager : ISessionManager
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ILogger _logger;
        private readonly IClientSessionService _clientSessionService;
        private IQueryable<Appointment> _appointments;

        public SessionManager(IRepository repository, ISaveEntityService saveEntityService, ILogger logger,
                              IClientSessionService clientSessionService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _logger = logger;
            _clientSessionService = clientSessionService;
        }

        public void CompleteAppointments()
        {
            _logger.LogInfo("Beginning CompleteAppointments Process.");

            IValidationManager validationManager = new ValidationManager(_repository);
            var appointments = _repository.Query<Appointment>(x => x.EndTime < DateTime.Now && !x.Completed).ToList();
            appointments.ToList().ForEachItem(x =>
                {
                    _logger.LogInfo("Session Mananger Processing aptId:{0}".ToFormat(x.EntityId));
                    _clientSessionService.SetSessionsForClients(x);
                    x.Completed = true;
                    x.Clients.Where(c =>c.ClientStatus!=null && c.ClientStatus.AdminAlerted).ForEachItem(c => c.ClientStatus.AdminAlerted = false);
                    validationManager = _saveEntityService.ProcessSave(x, validationManager);
                });
         // I found following three lines commented out.  no idea why, going to deploy and see wtf
        // here;s what I think happened. this was never live and the rollback/commit was toggled in session management.
            // var notification = validationManager.Finish();
            //var status = notification.Success?"Successful": "Failed :" +string.Join(", ", notification.Errors.Select(y=>y.ErrorMessage));
            //_logger.LogInfo("Session Manger Complete Appointments: " + status);
        }
    }
}

