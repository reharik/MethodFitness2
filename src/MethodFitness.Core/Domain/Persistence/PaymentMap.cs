using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class PaymentMap : DomainEntityMap<Payment>
    {
        public PaymentMap()
        {
            Map(x => x.FullHours);
            Map(x => x.FullHourTenPacks);
            Map(x => x.HalfHours);
            Map(x => x.HalfHourTenPacks);
            Map(x => x.Pairs);
            Map(x => x.PaymentTotal);
            References(x => x.Client);
        } 
    }
}