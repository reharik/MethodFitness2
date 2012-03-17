using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class UserMap : DomainEntityMap<User>
    {
        public UserMap()
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.BirthDate);
            Map(x => x.Email);
            Map(x => x.PhoneMobile);
            Map(x => x.SecondaryPhone);
            Map(x => x.Address1);
            Map(x => x.Address2);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.ZipCode);
            Map(x => x.Notes);
            Map(x => x.ImageUrl);
            References(x => x.UserLoginInfo);
            HasManyToMany(x => x.UserRoles).Access.CamelCaseField(Prefix.Underscore);
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

        public class TrainerMap : SubclassMap<Trainer>
        {
            public TrainerMap()
            {
                DiscriminatorValue("Trainer");
                Map(x => x.Color);
                Map(x => x.ClientRateDefault);
                HasManyToMany(x => x.Clients).Access.CamelCaseField(Prefix.Underscore);
                HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore);
                HasMany(x => x.TrainerClientRates).Access.CamelCaseField(Prefix.Underscore);
                HasMany(x => x.TrainerPayments).Access.CamelCaseField(Prefix.Underscore);
            }
        }
    }
}