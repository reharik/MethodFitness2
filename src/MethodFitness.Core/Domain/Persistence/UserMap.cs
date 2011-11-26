using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class UserMap : EntityMap<User>
    {
        public UserMap()
        {
            Map(x => x.UserId);
            Map(x => x.FirstName);
            Map(x => x.MiddleInitial);
            Map(x => x.LastName);
            Map(x => x.BirthDate);
            Map(x => x.StartPage);
            Map(x => x.SystemSupport);
            Map(x => x.Registering);
            HasMany(x => x.UserLoginInfos).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan();
        } 

        public class UserLoginInfoMap : DomainEntityMap<UserLoginInfo>
        {
            public UserLoginInfoMap()
            {
                Map(x => x.LoginName);
                Map(x => x.Password);
                Map(x => x.IsActive);
                Map(x => x.PasswordExpires); 
                Map(x => x.PasswordExpireDate);
                Map(x => x.Salt);
                Map(x => x.CanLogin);
                Map(x => x.LastVisitDate);
                Map(x => x.CanLoginFrom);
                Map(x => x.CanLoginTo);
                Map(x => x.ByPassToken);
                HasMany(x => x.UserSubscriptions).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan(); 
            }
        }

        public class UserSubscriptionMap : EntityMap<UserSubscription>
        {
            public UserSubscriptionMap()
            {
                Map(x => x.BeginDate);
                Map(x => x.ExpirationDate);
                Map(x => x.Approved);
                Map(x => x.AuthorizationCode);
                Map(x => x.CardNumber);
                Map(x => x.TransactionId);
            }
        }
    }
}