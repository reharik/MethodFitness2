using CC.Core.DomainTools;
using NHibernate;

namespace MF.Core.Domain.Tools
{
    public class MFUnitOfWork : UnitOfWork
    {
        public MFUnitOfWork(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        protected override void ReestablishFilters()
        {
            var enableDeletdFilter = CurrentSession.EnableFilter("IsDeletedConditionFilter");
            if (enableDeletdFilter != null)
                enableDeletdFilter.SetParameter("IsDeleted", false);
            base.ReestablishFilters();
        }
    }
}
