using MethodFitness.Core.Config;
using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class DomainEntityMap<DOMAINENTITY> : EntityMap<DOMAINENTITY> where DOMAINENTITY : DomainEntity
    {
        public DomainEntityMap()
        {
            Map(x => x.TenantId);
            Map(x => x.OrgId);
            ApplyFilter<TenantConditionFilter>("(TenantId= :TenantId Or TenantId= 1)");
            ApplyFilter<OrgConditionFilter>("(OrgId= :OrgId or OrgId = 1)");
        }
    }



    public class EntityMap<ENTITY> : ClassMap<ENTITY> where ENTITY : Entity
    {
        public EntityMap()
        {
            Id(x => x.EntityId);
            Map(x => x.CreateDate)
                .Default("(getdate())");
            Map(x => x.ChangeDate)
                //.Not.Nullable()
                .Default("(getdate())");
            Map(x => x.ChangedBy);
            Map(x => x.Archived);
            ApplyFilter<DeletedConditionFilter>("Archived= :Archived");
        }
    }
}
