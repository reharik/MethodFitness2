using System.Collections.Generic;
using CC.DataValidation.Attributes;

namespace MF.Core.Domain
{
    public class TrainerPayment:DomainEntity
    {
        [DoNotValidate]
        public virtual User Trainer { get; set; }
        public virtual double Total { get; set; }
        private IList<TrainerPaymentSessionItem> _trainerPaymentSessionItems = new List<TrainerPaymentSessionItem>();
        public virtual IEnumerable<TrainerPaymentSessionItem> TrainerPaymentSessionItems { get { return _trainerPaymentSessionItems; } }
        public virtual void RemoveTrainerPaymentSessionItem(TrainerPaymentSessionItem session)
        {
            _trainerPaymentSessionItems.Remove(session);
        }
        public virtual void AddTrainerPaymentSessionItem(TrainerPaymentSessionItem session)
        {
            if (_trainerPaymentSessionItems.Contains(session)) return;
            _trainerPaymentSessionItems.Add(session);
        }
    }

    public class TrainerPaymentSessionItem:DomainEntity
    {
        [DoNotValidate]
        public virtual Appointment Appointment { get; set; }
        [DoNotValidate]
        public virtual Client Client { get; set; }
        public virtual double AppointmentCost { get; set; }
        public virtual double TrainerPay { get; set; }
    }
}