using System;
using System.Collections.Generic;
using System.Linq;
using CC.Core.Core.DomainTools;
using CC.Core.Core.ValidationServices;
using CC.Core.Utilities;
using MF.Core.Domain;

namespace MF.Core.Services
{
    public interface IClientSessionService
    {
        void SetSessionsForClients(Appointment apt);
        void SettleChangesToPastAppointment(IEnumerable<int> newListOfClientIds, Appointment apt, string appointmentType);

        void RestoreSessionsFromAppointment(Appointment apt);
        void RestoreSessionToClient(Client client, Session session);
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
            if (client.Sessions.Any(s => s.Appointment == apt)) { return; }
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
            }
        }

        public virtual void RestoreSessionsFromAppointment(Appointment apt)
        {
            try
            {
                apt.Clients.ForEachItem(x =>
                    {
                        var session = x.Sessions.Where(s => s.Appointment!=null).FirstOrDefault(s => s.Appointment.EntityId == apt.EntityId);
                        RestoreSessionToClient(x, session);
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void RestoreSessionToClient(Client client, Session session)
        {
            _logger.LogInfo("Restoring Session to Client. ClientId:{0}, SessionId:{1}".ToFormat(client.EntityId, session != null ? session.EntityId : 0));
            if (session == null) return;
            if (session.InArrears)
            {
                client.RemoveSession(session);
                return;
            }

            var arrear = client.Sessions.FirstOrDefault(x => x.InArrears && x.AppointmentType == session.AppointmentType);
            if (arrear == null)
            {
                session.SessionUsed = false;
                session.Trainer = null;
                session.Appointment = null;
            }
            else
            {
                //switch app and trainer over to the session since the app that origionally
                //had the session will be deleted.
                session.Appointment = arrear.Appointment;
                session.Trainer = arrear.Trainer;
                client.RemoveSession(arrear);
            }
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
            clientIdsNewToAppointment.ForEachItem(x =>
                {
                    var client = _repository.Find<Client>(x);
                    SetSessionForClient(client, apt);
                    apt.AddClient(client);
                });
        }

        private void HandleRemovedClientsOnPastApt(IEnumerable<int> newListOfClientIds, Appointment apt)
        {
            var currentClientsIds = apt.Clients.Select(x => x.EntityId).ToList();
            var clientIdsRemovedFromAppointment = currentClientsIds.Except(newListOfClientIds);
            _logger.LogInfo("Removed clientIds for past apt. apt.id:{0}, clientIds:{1}".ToFormat(apt.EntityId, clientIdsRemovedFromAppointment));
            clientIdsRemovedFromAppointment.ForEachItem(x =>
            {
                var client = apt.Clients.FirstOrDefault(c => c.EntityId == x);
                if (client != null)
                {
                    var session = client.Sessions.FirstOrDefault(s => s.Appointment.EntityId == apt.EntityId);
                    apt.RemoveClient(client);
                    RestoreSessionToClient(client,session);
                    _saveEntityService.ProcessSave(client);
                }
            });
        }

        private bool HandleChangeOfAptTypeInPastApt(string appointmentType, Appointment apt)
        {
            if (apt.AppointmentType == appointmentType){return false;}

            RestoreSessionsFromAppointment(apt);
            apt.Completed = false;
            return true;
        }
    }
}