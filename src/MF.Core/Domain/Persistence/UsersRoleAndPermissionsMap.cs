namespace MF.Core.Domain.Persistence
{
    public class UsersRoleAndPermissionsMap : DomainEntityMap<UsersRoleAndPermissions>
    {
        public UsersRoleAndPermissionsMap()
        {
            References(x => x.LocationId).Column("`LocationId`");
            References(x => x.Trainer).Column("`UserId`");
            References(x => x.Role).Column("`UserRoleId`");
        }
    }
}