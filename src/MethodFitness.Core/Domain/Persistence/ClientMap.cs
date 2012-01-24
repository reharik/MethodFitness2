using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class ClientMap : DomainEntityMap<Client>
    {
        public ClientMap()
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.BirthDate);
            Map(x => x.Email);
            Map(x => x.MobilePhone);
            Map(x => x.SecondaryPhone);
            Map(x => x.Address1);
            Map(x => x.Address2);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.ZipCode);
            Map(x => x.Notes);
            Map(x => x.ImageUrl);
            References(x => x.SessionRates);
            HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore);
            HasMany(x => x.Payments).Access.CamelCaseField(Prefix.Underscore);

        } 
    }
}