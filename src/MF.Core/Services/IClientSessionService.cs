using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
            _logger.LogDebug("Client name for appt {0}: {1}".ToFormat(apt.EntityId, client.FullNameFNF));
            var sessions = client.Sessions.Where(s => !s.SessionUsed && s.AppointmentType == apt.AppointmentType);
            var session = new Session();
            if (sessions.Any())
            {
                session = sessions.OrderBy(s => s.CreatedDate).First();
                session.Appointment = apt;
                session.Trainer = apt.Trainer;
                session.SessionUsed = true;
                _logger.LogDebug("Updating session: {0}, for appointment: {1}, and client: {2}", session.EntityId, apt.EntityId, client.FullNameFNF);
            }
            else
            {
                session = new Session
                {
                    Appointment = apt,
                    Trainer = apt.Trainer,
                    InArrears = true,
                    AppointmentType = apt.AppointmentType,
                    SessionUsed = true
                };
                client.AddSession(session);
                _logger.LogDebug("Added InArrears session: {0} for Client: {1} with sessions: {2}", session.EntityId, client.EntityId, String.Join(",", client.Sessions.OrderByDescending(x=>x.CreatedDate).Take(10).Select(x=>x.EntityId).ToList()));
            }
            _logger.LogDebug("Proceessed Appointment: {0}, Session: {1}, client: {2}".ToFormat(apt.EntityId,session.EntityId,client.FullNameFNF));
        }

        public virtual void RestoreSessionsFromAppointment(Appointment apt)
        {
            try
            {
                apt.Clients.ForEachItem(x =>
                    {
                        var session = x.Sessions.Where(s => s.Appointment!=null).FirstOrDefault(s => s.Appointment.EntityId == apt.EntityId);
                        if (session != null)
                        {
                            _logger.LogDebug("Restoring session: {0}, to Client: {1} for Appointment: {2}",session.EntityId,x.EntityId,apt.EntityId);
                            RestoreSessionToClient(x, session);
                        }
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void RestoreSessionToClient(Client client, Session session)
        {
            if (session == null) return;
            if (session.InArrears)
            {
                client.RemoveSession(session);
                _logger.LogDebug("Removing InArrears Session: {0} for Client: {1}",session.EntityId,client.EntityId);
                return;
            }

            var arrear = client.Sessions.FirstOrDefault(x => x.InArrears && x.AppointmentType == session.AppointmentType);
            if (arrear == null)
            {
                session.SessionUsed = false;
                session.Trainer = null;
                session.Appointment = null;
                _logger.LogDebug("Restoring Session: {0} to unused for Client: {1}", session.EntityId, client.EntityId);
            }
            else
            {
                //switch app and trainer over to the session since the app that origionally
                //had the session will be deleted.
                _logger.LogDebug("Moving inarrears session:{0} for appointmentId: {1} to session: {2} with original appointment: {3}"
                    .ToFormat(arrear.EntityId,arrear.Appointment.EntityId,session.EntityId,session.Appointment!=null?session.Appointment.EntityId:0));
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
                    _logger.LogDebug("SessionId: {0} being updated because client removed from appointmentId: {1}".ToFormat(session.EntityId,apt.EntityId));
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