using CC.Core.DomainTools;
using NHibernate;
using StructureMap;

namespace MethodFitness.Core.Domain.Tools
{
    public class NoInterceptorNoFiltersUnitOfWork : UnitOfWork
    {
        public NoInterceptorNoFiltersUnitOfWork()
        {
            _session = ObjectFactory.Container.GetInstance<ISession>("NoInterceptorNoFilters");
        }
    }

}
