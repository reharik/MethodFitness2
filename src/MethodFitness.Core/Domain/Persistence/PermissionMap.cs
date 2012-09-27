using CC.Security.Model;
using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class PermissionMap : ClassMap<Permission>
    {
        public PermissionMap()
        {
            Cache.ReadWrite().Region("cc-security");
            Id(x => x.EntityId);

            Map(x => x.Allow).Not.Nullable();
            Map(x => x.Description).Length(1000);
            Map(x => x.Level).Not.Nullable();
            References(x => x.Operation).Not.Nullable();
            References(x => x.User);
            References(x => x.UsersGroup);
        }

    }
}