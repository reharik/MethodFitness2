namespace MF.Core.Domain.Persistence
{
    public class BaseSessionRateMap : DomainEntityMap<BaseSessionRate>
    {
        public BaseSessionRateMap()
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