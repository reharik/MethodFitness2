using System.Collections.Generic;

namespace MethodFitness.Core.Domain
{
    public class TrainerPayment:DomainEntity
    {
        public virtual User Trainer { get; set; }
        public virtual double Total { get; set; }
        private IList<TrainerPaymentSessionItem> _sessions = new List<TrainerPaymentSessionItem>();
        public virtual void EmptyTrainerPaymentSessionItems() { _sessions.Clear(); }
        public virtual IEnumerable<TrainerPaymentSessionItem> TrainerPaymentSessionItems { get { return _sessions; } }
        public virtual void RemoveTrainerPaymentSessionItem(TrainerPaymentSessionItem session)
        {
            _sessions.Remove(session);
        }
        public virtual void AddTrainerPaymentSessionItem(TrainerPaymentSessionItem session)
        {
            if (_sessions.Contains(session)) return;
            _sessions.Add(session);
        }
    }

    public class TrainerPaymentSessionItem:DomainEntity
    {
        public virtual Appointment Appointment { get; set; }
        public virtual Client Client { get; set; }
        public virtual double AppointmentCost { get; set; }
        public virtual double TrainerPay { get; set; }
    }
}