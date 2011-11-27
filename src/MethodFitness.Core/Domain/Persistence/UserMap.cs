using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class UserMap : DomainEntityMap<User>
    {
        public UserMap()
        {
            Map(x => x.UserId);
            Map(x => x.FirstName);
            Map(x => x.MiddleInitial);
            Map(x => x.LastName);
            Map(x => x.BirthDate);
            References(x => x.UserLoginInfo);
        } 

        public class UserLoginInfoMap : DomainEntityMap<UserLoginInfo>
        {
            public UserLoginInfoMap()
            {
                Map(x => x.LoginName);
                Map(x => x.Password);
                Map(x => x.Salt);
                Map(x => x.CanLogin);
                Map(x => x.LastVisitDate);
                Map(x => x.ByPassToken);
            }
        }
    }
}