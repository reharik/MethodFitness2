using CC.Core.Domain;
using MethodFitness.Core.Config;
using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class DomainEntityMap<DOMAINENTITY> : EntityMap<DOMAINENTITY> where DOMAINENTITY : DomainEntity
    {
        public DomainEntityMap()
        {
            Map(x => x.CompanyId);
            ApplyFilter<CompanyConditionFilter>("(CompanyId= :CompanyId)");
        }
    }



    public class EntityMap<ENTITY> : ClassMap<ENTITY> where ENTITY : Entity
    {
        public EntityMap()
        {
            Id(x => x.EntityId);
            Map(x => x.CreatedDate)
                .Default("(getdate())");
            Map(x => x.ChangedDate)
                //.Not.Nullable()
                .Default("(getdate())");
            Map(x => x.ChangedBy);
            Map(x => x.IsDeleted);
            ApplyFilter<DeletedConditionFilter>("Archived= :Archived");
        }
    }
}
