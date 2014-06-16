namespace MF.Core.Domain.Persistence
{
    public class UserRoleMap : EntityMap<UserRole>
    {
        public UserRoleMap()
        {
            Map(x => x.Name);
            Map(x => x.Description);
        } 
    }
}