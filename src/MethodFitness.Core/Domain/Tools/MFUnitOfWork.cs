using CC.Core.DomainTools;
using NHibernate;

namespace KnowYourTurf.Core.Domain.Tools
{
    public class MFUnitOfWork : UnitOfWork
    {
        private ISession _session;

        public MFUnitOfWork(ISession session)
            : base(session)
        {
            _session = session;
            var enableDeletdFilter = _session.EnableFilter("IsDeletedConditionFilter");
            var enableStatusFilter = _session.EnableFilter("StatusConditionFilter");
            if (enableDeletdFilter != null)
                enableDeletdFilter.SetParameter("IsDeleted", false);
            if (enableStatusFilter != null)
                enableStatusFilter.SetParameter("Status", "Active");
        }
    }
}