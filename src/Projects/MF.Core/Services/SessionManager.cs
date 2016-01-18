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
          
            var appointment = _repository.Query<Appointment>(x => x.EndTime < DateTime.Now && !x.Completed).Take(1).FirstOrDefault();
            while (appointment != null)
            {
                _logger.LogInfo("Session Mananger Processing aptId:{0}".ToFormat(appointment.EntityId));
                _clientSessionService.SetSessionsForClients(appointment);
                appointment.Completed = true;
                appointment.Clients.Where(c => c.ClientStatus != null && c.ClientStatus.AdminAlerted).ForEachItem(c => c.ClientStatus.AdminAlerted = false);
                validationManager = _saveEntityService.ProcessSave(appointment, validationManager);
                _logger.LogDebug("about to complete session");
                validationManager.Finish();
                appointment = _repository.Query<Appointment>(x => x.EndTime < DateTime.Now && !x.Completed).Take(1).FirstOrDefault();
            }
        }
    }
}

