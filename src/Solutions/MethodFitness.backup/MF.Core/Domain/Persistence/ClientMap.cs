using FluentNHibernate.Mapping;

namespace MF.Core.Domain.Persistence
{
    public class ClientMap : DomainEntityMap<Client>
    {
        public ClientMap()
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.BirthDate);
            Map(x => x.StartDate);
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
            Map(x => x.Source);
            Map(x => x.SourceOther);
            Map(x => x.Archived);
            References(x => x.SessionRates);
            References(x => x.ClientStatus);
            HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan();
            HasMany(x => x.Payments).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan();

        }
    }
}