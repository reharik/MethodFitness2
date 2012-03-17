using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class TrainerPaymentMap : DomainEntityMap<TrainerPayment>
    {
        public TrainerPaymentMap()
        {
            Map(x => x.Total);
            References(x => x.Trainer).Column("UserId");
            HasMany(x => x.TrainerPaymentSessionItems).Access.CamelCaseField(Prefix.Underscore);
        } 
    }

    public class TrainerPaymentSessionItemMap : DomainEntityMap<TrainerPaymentSessionItem>
    {
        public TrainerPaymentSessionItemMap()
        {
            Map(x => x.AppointmentCost);
            Map(x => x.TrainerPay);
            References(x => x.Client);
            References(x => x.Appointment);
        }
    }
}