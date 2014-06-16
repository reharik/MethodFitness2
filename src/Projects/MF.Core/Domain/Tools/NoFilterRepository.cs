using CC.Core.DomainTools;
using StructureMap;

namespace MF.Core.Domain.Tools
{
    public class NoFilterRepository : Repository
    {
        public NoFilterRepository()
        {
            _unitOfWork = ObjectFactory.Container.GetInstance<IUnitOfWork>();
            _unitOfWork.Initialize();
        }
    }
}