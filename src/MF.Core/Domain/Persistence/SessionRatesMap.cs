namespace MF.Core.Domain.Persistence
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
            Map(x => x.PairTenPack);
        } 
    }
}