using CC.Core.DomainTools;
using StructureMap;

namespace MethodFitness.Core.Domain.Tools
{
    public class NoInterceptorNoFiltersRepository : Repository
    {
        public NoInterceptorNoFiltersRepository()
        {
            _unitOfWork = ObjectFactory.Container.GetInstance<IUnitOfWork>("NoInterceptorNoFilters");
            _unitOfWork.Initialize();
        }
    }
}