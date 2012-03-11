using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class TrainerClientRateMap : DomainEntityMap<TrainerClientRate>
    {
        public TrainerClientRateMap()
        {
            Map(x => x.Percent).Column("`Percent`");
            References(x => x.User);
            References(x => x.Client);
        } 
    }
}