using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class TrainerSessionVerificationMap : DomainEntityMap<TrainerSessionVerification>
    {
        public TrainerSessionVerificationMap()
        {
            Map(x => x.Total);
            References(x => x.Trainer).Column("TrainerId");
            HasMany(x => x.TrainerApprovedSessionItems).Access.CamelCaseField(Prefix.Underscore);
        } 
    }

}