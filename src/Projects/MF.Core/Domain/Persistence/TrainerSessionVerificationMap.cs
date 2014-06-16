using FluentNHibernate.Mapping;

namespace MF.Core.Domain.Persistence
{
    public class TrainerSessionVerificationMap : DomainEntityMap<TrainerSessionVerification>
    {
        public TrainerSessionVerificationMap()
        {
            Map(x => x.Total);
            References(x => x.Trainer);
            HasMany(x => x.TrainerApprovedSessionItems).Access.CamelCaseField(Prefix.Underscore);
        } 
    }

}