using System;
using System.Collections.Generic;
using System.Linq;
using CC.Core;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Localization;
using CC.Security.Interfaces;
using Castle.Components.Validator;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using MethodFitness.Core.Enumerations;
using xVal.ServerSide;

namespace MethodFitness.Core.Domain
{
    public class Appointment : DomainEntity, IPersistableObject
    {
        [ValidateNonEmpty]
        public virtual DateTime? Date { get; set; }
        [ValidateNonEmpty]
        public virtual DateTime? EndTime { get; set; }
        [ValidateNonEmpty]
        public virtual DateTime? StartTime { get; set; }
        [ValidateNonEmpty]
        public virtual Location Location { get; set; }
        [ValidateNonEmpty]
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
        
        public virtual void SettleChangesToPastAppointment(IEnumerable<int> newListOfClientIds, string appointmentType,IRepository repository)
        {
            if (IsNew()) return;
            if (HandleChangeOfAptTypeInPastApt(appointmentType)) return;
            HandleChangeOfClientsOnPastApt(newListOfClientIds, repository);
        }

        private void HandleChangeOfClientsOnPastApt(IEnumerable<int> newListOfClientIds, IRepository repository)
        {
            var currentClientsIds = Clients.Select(x => x.EntityId);
            var clientIdsNewToAppointment = currentClientsIds.Except(newListOfClientIds);

            if (clientIdsNewToAppointment.Any())
            {
                clientIdsNewToAppointment.ForEachItem(x => SetSessionForClient(repository.Find<Client>(x)));
            }
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