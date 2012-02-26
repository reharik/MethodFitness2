using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class TrainerClientRateMap : DomainEntityMap<TrainerClientRate>
    {
        public TrainerClientRateMap()
        {
            Map(x => x.Percent);
            References(x => x.Trainer);
            References(x => x.Client);
        } 
    }
}