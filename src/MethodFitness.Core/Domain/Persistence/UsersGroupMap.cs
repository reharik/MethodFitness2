using CC.Security.Model;
using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class UsersGroupMap : ClassMap<UsersGroup>
    {
        public UsersGroupMap()
        {
            Cache.ReadWrite().Region("cc-security");
            Id(x => x.EntityId);

            Map(x => x.Name).Not.Nullable().Length(255).Unique();
            Map(x => x.Description).Length(1000);
            References(x => x.Parent);
            HasMany(x => x.DirectChildren)
                .KeyColumn("Parent")
                .LazyLoad()
                .Inverse()
                .Cache.ReadWrite()
                .Region("cc-security");
            HasManyToMany(x => x.Users).Table("UsersToUsersGroup").LazyLoad().Cache.ReadWrite().Region("cc-security");
            HasManyToMany(x => x.AllChildren)
                .Table("UsersGroupsHierarchy")
                .ChildKeyColumn("ChildGroup")
                .LazyLoad()
                .Cache.ReadWrite()
                .Region("cc-security");
            HasManyToMany(x => x.AllParents)
                .Table("UsersGroupsHierarchy")
                .ChildKeyColumn("ParentGroup")
                .LazyLoad()
                .Cache.ReadWrite()
                .Region("cc-security");
        }

    }
}