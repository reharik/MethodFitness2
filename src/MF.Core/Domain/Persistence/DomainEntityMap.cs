using CC.Core.Core.Domain;
using FluentNHibernate.Mapping;
using MF.Core.Config;

namespace MF.Core.Domain.Persistence
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
            Map(x => x.CreatedDate);
            Map(x => x.ChangedDate);
            References(x => x.ChangedBy).LazyLoad().Class<User>();
            References(x => x.CreatedBy).LazyLoad().Class<User>();
            Map(x => x.IsDeleted);
            ApplyFilter<IsDeletedConditionFilter>("IsDeleted= :IsDeleted");
        }
    }
}
