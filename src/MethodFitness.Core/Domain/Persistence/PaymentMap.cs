using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class PaymentMap : DomainEntityMap<Payment>
    {
        public PaymentMap()
        {
            Map(x => x.FullHour);
            Map(x => x.FullHourTenPack);
            Map(x => x.HalfHour);
            Map(x => x.HalfHourTenPack);
            Map(x => x.Pair);
            Map(x => x.PaymentTotal);
            Map(x => x.FullHourPrice);
            Map(x => x.FullHourTenPackPrice);
            Map(x => x.HalfHourPrice);
            Map(x => x.HalfHourTenPackPrice);
            Map(x => x.PairPrice);
            Map(x => x.PairTenPack);
            Map(x => x.PairTenPackPrice);
            References(x => x.Client);
        } 
    }
}