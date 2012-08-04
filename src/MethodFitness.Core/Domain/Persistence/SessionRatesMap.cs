using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class SessionRatesMap : DomainEntityMap<SessionRates>
    {
        public SessionRatesMap()
        {
            Map(x => x.FullHour);
            Map(x => x.HalfHour);
            Map(x => x.FullHourTenPack);
            Map(x => x.HalfHourTenPack);
            Map(x => x.Pair);
        } 
    }
}