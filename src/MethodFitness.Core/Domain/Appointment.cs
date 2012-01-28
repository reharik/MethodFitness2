using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.Validator;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Localization;
using MethodFitness.Core.Services;
using Rhino.Security.Interfaces;
using xVal.ServerSide;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class Appointment:DomainEntity
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
        [ValueOf(typeof(AppointmentLength))]
        public virtual string Length { get; set; }
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
  
        public virtual Notification CheckPermissions(User user, IAuthorizationService authorizationService, Notification notification)
        {
            var convertTime = TimeZoneInfo.ConvertTime(DateTime.Now,TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (StartTime < convertTime && !authorizationService.IsAllowed(user, "/Calendar/CanEnterRetroactiveAppointments"))
            {
                notification.Success = false;
                notification.Message = CoreLocalizationKeys.YOU_CAN_NOT_CREATE_RETROACTIVE_APPOINTMENTS.ToString();
            }
            return notification;

        } 
        public virtual Notification CheckForClients(Notification notification)
        {
            if (!Clients.Any())
            {
                notification = new Notification { Success = false };
                notification.Errors = new List<ErrorInfo> { new ErrorInfo(CoreLocalizationKeys.CLIENTS.ToString(), CoreLocalizationKeys.SELECT_AT_LEAST_ONE_CLIENT.ToString()) };
            }
            return notification;
        }
        public virtual void SetSessionsForClients()
        {
            Clients.Each(x =>
            {
                var sessions =
                    x.Sessions.Where(
                        s => s.Appointment == null && s.AppointmentType == Length);
                if (sessions.Any())
                {
                    var session = sessions.OrderBy(s => s.Date).First();
                    session.Appointment = this;
                    session.Trainer = Trainer;
                }
                else
                {
                    var session = new Session
                    {
                        Appointment = this,
                        Trainer = Trainer,
                        InArrears = true,
                        AppointmentType = Length
                    };
                    x.AddSession(session);
                    AddSession(session);
                }

            });
        }

    }
}