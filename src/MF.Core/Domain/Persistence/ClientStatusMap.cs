using FluentNHibernate.Mapping;

namespace MF.Core.Domain.Persistence
{
    public class ClientStatusMap : DomainEntityMap<ClientStatus>
    {
        public ClientStatusMap()
        {
            Map(x => x.AdminAlerted);
            Map(x => x.ClientContacted);
        }
    }
}