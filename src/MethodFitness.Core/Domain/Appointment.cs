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
using MethodFitness.Web.Areas.Schedule.Controllers;
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
            Clients.ForEachItem(x =>
            {
                var sessions = x.Sessions.Where(s => s.Appointment == null && s.AppointmentType == AppointmentType);
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
                    x.AddSession(session);
                    AddSession(session);
                }
            });
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
        //christ this was a hard one to name. 
        // if there were changes to type or clients return the sessions to client.
        // if sessions were returned to clients we need to re apply them for this appointment.
        public virtual bool CheckForChangesAndReturnNeedToSetSessions(IEnumerable<TokenInputDto> selectedItems, string appointmentType)
        {
            if (IsNew()) return true;
            var currentClientsIds = Clients.Select(x => x.EntityId);
            var newClientsIds = selectedItems.Select(x => Int32.Parse(x.id));
            if(AppointmentType != appointmentType || currentClientsIds.Except(newClientsIds).Any())
            {
                RestoreSessionsToClients();
                return true;
            }
            return false;
        }
    }
}