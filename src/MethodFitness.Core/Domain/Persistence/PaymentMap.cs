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
            Map(x => x.FullHoursPrice);
            Map(x => x.FullHourTenPacksPrice);
            Map(x => x.HalfHoursPrice);
            Map(x => x.HalfHourTenPacksPrice);
            Map(x => x.PairsPrice);
            References(x => x.Client);
        } 
    }
}