using CC.Core.Core.DomainTools;
using StructureMap;

namespace MF.Core.Domain.Tools
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