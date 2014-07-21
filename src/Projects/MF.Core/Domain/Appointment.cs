using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CC.Core;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Localization;
using CC.Core.Services;
using CC.Core.ValidationServices;
using CC.DataValidation.Attributes;
using CC.Utility;
using MF.Core.Domain.Tools.CustomAttributes;
using MF.Core.Enumerations;

namespace MF.Core.Domain
{
    public class Appointment : DomainEntity
    {
        [Required]
        public virtual DateTime? Date { get; set; }
        [Required]
        public virtual DateTime? EndTime { get; set; }
        [Required]
        public virtual DateTime? StartTime { get; set; }
        [Required]
        public virtual Location Location { get; set; }
        [DoNotValidate]
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
        [DoNotValidate]
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
//        private IList<Session> _sessions = new List<Session>();
//        public virtual void EmptySessions() { _sessions.Clear(); }
//        [DoNotValidate]
//        public virtual IEnumerable<Session> Sessions { get { return _sessions; } }
//        public virtual void RemoveSession(Session session)
//        {
//            _sessions.Remove(session);
//        }
//        public virtual void AddSession(Session session)
//        {
//            if (_sessions.Contains(session)) return;
//            _sessions.Add(session);
//        }
        #endregion
        

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

    }
}