using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class CompanyMap : DomainEntityMap<Company>
    {
        public CompanyMap()
        {
            Map(x => x.Name);
            Map(x => x.Description);
        } 
    }
}