using System;
using System.Collections.Generic;
using Castle.Components.Validator;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools.CustomAttributes;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class Appointment:DomainEntity
    {
        [ValidateNonEmpty]
        public virtual DateTime? ScheduledDate { get; set; }
        [ValidateNonEmpty]
        public virtual DateTime? ScheduledEndTime { get; set; }
        [ValidateNonEmpty]
        public virtual DateTime? ScheduledStartTime { get; set; }
        [ValidateNonEmpty]
        public virtual Location Location { get; set; }
        [ValidateNonEmpty]
        public virtual User Trainer { get; set; }
        [TextArea]
        public virtual string Notes { get; set; }
       

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

        #endregion

    }
}