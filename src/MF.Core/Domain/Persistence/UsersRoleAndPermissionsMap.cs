namespace MF.Core.Domain.Persistence
{
    public class UsersRoleAndPermissionsMap : DomainEntityMap<UsersRoleAndPermissions>
    {
        public UsersRoleAndPermissionsMap()
        {
            HasManyToMany(x => x.Locations).Table("UsersRoleAndPermissions_Location");
            References(x => x.Trainer).Column("`UserId`");
            References(x => x.Role).Column("`UserRoleId`");
        }
    }
}