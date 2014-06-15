using CC.Core.DomainTools;
using NHibernate;

namespace MethodFitness.Core.Domain.Tools
{
    public class MFUnitOfWork : UnitOfWork
    {
        private ISession _session;

        public MFUnitOfWork(ISession session)
            : base(session)
        {
            _session = session;
            var enableDeletdFilter = _session.EnableFilter("IsDeletedConditionFilter");
            if (enableDeletdFilter != null)
                enableDeletdFilter.SetParameter("IsDeleted", false);
        }
    }
}
