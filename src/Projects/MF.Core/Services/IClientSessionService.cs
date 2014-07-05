using System.Collections.Generic;
using System.Linq;
using CC.Core.DomainTools;
using CC.Core.ValidationServices;
using CC.Utility;
using MF.Core.Domain;
using MF.Core.Enumerations;

namespace MF.Core.Services
{
    public interface IClientSessionService
    {
        void SetSessionsForClients(Appointment apt);
        void SettleChangesToPastAppointment(IEnumerable<int> newListOfClientIds, Appointment apt, string appointmentType);

        void RestoreSessionsToClients(IEnumerable<Session> sessions);
    }

    public class ClientSessionService : IClientSessionService
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ILogger _logger;

        public ClientSessionService(IRepository repository,
            ISaveEntityService saveEntityService,
            ILogger logger)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _logger = logger;
        }

        public virtual void SetSessionsForClients(Appointment apt)
        {
            apt.Clients.ForEachItem(x=>SetSessionForClient(x, apt));
        }

        private void SetSessionForClient(Client client, Appointment apt)
        {
            var sessions = client.Sessions.Where(s => !s.SessionUsed && s.AppointmentType == apt.AppointmentType);
            if (sessions.Any())
            {
                var session = sessions.OrderBy(s => s.CreatedDate).First();
                session.Appointment = apt;
                session.Trainer = apt.Trainer;
                session.SessionUsed = true;
            }
            else
            {
                var session = new Session
                {
                    Appointment = apt,
                    Trainer = apt.Trainer,
                    InArrears = true,
                    AppointmentType = apt.AppointmentType,
                    SessionUsed = true
                };
                client.AddSession(session);
                apt.AddSession(session);
            }
        }

        public virtual void RestoreSessionsToClients(IEnumerable<Session> sessions)
        {
            sessions.ForEachItem(x =>
                {
                    _logger.LogInfo("Restoring Session to Client. ClientId:{0}, SessionId:{1}".ToFormat(x.Client.EntityId,x.EntityId));
                    x.Client.RestoreSession(x);
                });
        }

        public virtual void SettleChangesToPastAppointment(IEnumerable<int> newListOfClientIds, Appointment apt, string appointmentType)
        {
            _logger.LogInfo("Handling Change to Appointment in the past. apt.id:{0}".ToFormat(apt.EntityId));
            if (HandleChangeOfAptTypeInPastApt(appointmentType, apt)) return;
            HandleRemovedClientsOnPastApt(newListOfClientIds, apt);
            HandleNewClientsOnPastApt(newListOfClientIds, apt);
        }

        private void HandleNewClientsOnPastApt(IEnumerable<int> newListOfClientIds, Appointment apt)
        {
            var currentClientsIds = apt.Clients.Select(x => x.EntityId);
            var clientIdsNewToAppointment = newListOfClientIds.Except(currentClientsIds);
            _logger.LogInfo("New clientIds for past apt. apt.id:{0}, clientIds:{1}".ToFormat(apt.EntityId,clientIdsNewToAppointment));
            clientIdsNewToAppointment.ForEachItem(x => SetSessionForClient(_repository.Find<Client>(x),apt));
        }

        private void HandleRemovedClientsOnPastApt(IEnumerable<int> newListOfClientIds, Appointment apt)
        {
            var currentClientsIds = apt.Clients.Select(x => x.EntityId);
            var clientIdsRemovedFromAppointment = currentClientsIds.Except(newListOfClientIds);
            _logger.LogInfo("Removed clientIds for past apt. apt.id:{0}, clientIds:{1}".ToFormat(apt.EntityId, clientIdsRemovedFromAppointment));
            clientIdsRemovedFromAppointment.ForEachItem(x =>
            {
                var session = apt.Sessions.FirstOrDefault(s => s.Client.EntityId == x);
                var client = apt.Clients.FirstOrDefault(c => c.EntityId == x);
                apt.RemoveSession(session);
                client.RestoreSession(session);
                _saveEntityService.ProcessSave(client);
            });
        }

        private bool HandleChangeOfAptTypeInPastApt(string appointmentType, Appointment apt)
        {
            if (apt.AppointmentType == appointmentType){return false;}

            RestoreSessionsToClients(apt.Sessions);
            apt.Completed = false;
            return true;
        }
    }
}