using FluentNHibernate.Mapping;

namespace MF.Core.Domain.Persistence
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
            Map(x => x.Color);
            Map(x => x.ClientRateDefault);
            Map(x => x.Archived);
            References(x => x.UserLoginInfo);
            HasMany(x => x.Appointments).Access.CamelCaseField(Prefix.Underscore).KeyColumn("TrainerId");
            HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore).KeyColumn("TrainerId");
            HasMany(x => x.TrainerClientRates).Access.CamelCaseField(Prefix.Underscore).KeyColumn("TrainerId");
            HasMany(x => x.TrainerPayments).Access.CamelCaseField(Prefix.Underscore).KeyColumn("TrainerId");
            HasMany(x => x.TrainerSessionVerifications).Access.CamelCaseField(Prefix.Underscore).KeyColumn("TrainerId");
            HasManyToMany(x => x.UserRoles).Access.CamelCaseField(Prefix.Underscore);
            HasManyToMany(x => x.Clients).Access.CamelCaseField(Prefix.Underscore);

        }

        public class UserLoginInfoMap : DomainEntityMap<UserLoginInfo>
        {
            public UserLoginInfoMap()
            {
                Map(x => x.LoginName);
                Map(x => x.Password);
                Map(x => x.Salt);
                Map(x => x.LastVisitDate);
                Map(x => x.ByPassToken);
            }
        }
    }
}