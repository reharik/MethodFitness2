namespace MF.Core.Domain.Persistence
{
    public class TrainerClientRateMap : DomainEntityMap<TrainerClientRate>
    {
        public TrainerClientRateMap()
        {
            Map(x => x.Percent).Column("`Percent`");
            References(x => x.Trainer);
            References(x => x.Client);
        } 
    }
}