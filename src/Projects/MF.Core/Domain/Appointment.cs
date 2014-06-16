using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CC.Core;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Localization;
using CC.Core.Services;
using MF.Core.Domain.Tools.CustomAttributes;
using MF.Core.Enumerations;

namespace MF.Core.Domain
{
    public class Appointment : DomainEntity, IPersistableObject
    {
        [Required]
        public virtual DateTime? Date { get; set; }
        [Required]
        public virtual DateTime? EndTime { get; set; }
        [Required]
        public virtual DateTime? StartTime { get; set; }
        [Required]
        public virtual Location Location { get; set; }
        [Required]
        public virtual User Trainer { get; set; }
        [TextArea]
        public virtual string Notes { get; set; }
        [ValueOf(typeof(AppointmentType))]
        public virtual string AppointmentType { get; set; }
        public virtual bool Completed { get; set; }
        
        #region Collections
        private IList<Client> _clients = new List<Client>();
        public virtual void EmptyClients() { _clients.Clear(); }
        public virtual IEnumerable<Client> Clients { get { return _clients; } }
        public virtual void RemoveClient(Client client)
        {
            _clients.Remove(client);
        }
        public virtual void AddClient(Client client)
        {
            if (_clients.Contains(client)) return;
            _clients.Add(client);
        }
        private IList<Session> _sessions = new List<Session>();
        public virtual void EmptySessions() { _sessions.Clear(); }
        public virtual IEnumerable<Session> Sessions { get { return _sessions; } }
        public virtual void RemoveSession(Session session)
        {
            _sessions.Remove(session);
        }
        public virtual void AddSession(Session session)
        {
            if (_sessions.Contains(session)) return;
            _sessions.Add(session);
        }
        #endregion
        
        public virtual void SetSessionsForClients()
        {
            Clients.ForEachItem(SetSessionForClient);
        }

        private void SetSessionForClient(Client client)
        {
            var sessions = client.Sessions.Where(s => !s.SessionUsed && s.AppointmentType == AppointmentType);
            if (sessions.Any())
            {
                var session = sessions.OrderBy(s => s.CreatedDate).First();
                session.Appointment = this;
                session.Trainer = Trainer;
                session.SessionUsed = true;
            }
            else
            {
                var session = new Session
                    {
                        Appointment = this,
                        Trainer = Trainer,
                        InArrears = true,
                        AppointmentType = AppointmentType,
                        SessionUsed = true
                    };
                client.AddSession(session);
                AddSession(session);
            }
        }

        public virtual void RestoreSessionsToClients()
        {
            Sessions.ForEachItem(x => x.Client.RestoreSession(x));
        }

        public override Entity CloneSelf()
        {
            var appointment = new Appointment()
                                  {
                                      CompanyId = CompanyId,
                                      Date = Date,
                                      EndTime = EndTime,
                                      StartTime = StartTime,
                                      Location = Location,
                                      Trainer = Trainer,
                                      Notes = Notes,
                                      AppointmentType = AppointmentType,
                                  };
            _clients.ForEachItem(appointment.AddClient);
            return appointment;
        }

        public virtual void SettleChangesToPastAppointment(IEnumerable<int> newListOfClientIds,
                                                            string appointmentType,
                                                            IRepository repository,
                                                            ISaveEntityService saveEntityService)
        {
            if (IsNew()) return;
            if (HandleChangeOfAptTypeInPastApt(appointmentType)) return;
            HandleRemovedClientsOnPastApt(newListOfClientIds, saveEntityService);
            HandleNewClientsOnPastApt(newListOfClientIds, repository);
        }

        private void HandleNewClientsOnPastApt(IEnumerable<int> newListOfClientIds, IRepository repository)
        {
            var currentClientsIds = Clients.Select(x => x.EntityId);
            var clientIdsNewToAppointment = newListOfClientIds.Except(currentClientsIds);
            clientIdsNewToAppointment.ForEachItem(x => SetSessionForClient(repository.Find<Client>(x)));
        }

        private void HandleRemovedClientsOnPastApt(IEnumerable<int> newListOfClientIds, ISaveEntityService saveEntityService)
        {
            var currentClientsIds = Clients.Select(x => x.EntityId);
            var clientIdsRemovedFromAppointment = currentClientsIds.Except(newListOfClientIds);

            clientIdsRemovedFromAppointment.ForEachItem(x =>
                {
                    var session = Sessions.FirstOrDefault(s => s.Client.EntityId == x);
                    var client = Clients.FirstOrDefault(c => c.EntityId == x);
                    RemoveSession(session);
                    client.RestoreSession(session);
                    saveEntityService.ProcessSave(client);
                });
        }

        private bool HandleChangeOfAptTypeInPastApt(string appointmentType)
        {
            if (AppointmentType != appointmentType)
            {
                RestoreSessionsToClients();
                Completed = false;
                return true;
            }
            return false;
        }
    }
}