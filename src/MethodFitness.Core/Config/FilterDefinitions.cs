using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Config
{
    public class OrgConditionFilter : FilterDefinition
    {
        public OrgConditionFilter()
        {
            WithName("OrgConditionFilter")
                .AddParameter("OrgId", NHibernate.NHibernateUtil.Int32);
        }
    }
    public class TenantConditionFilter : FilterDefinition
    {
        public TenantConditionFilter()
        {
            WithName("TenantConditionFilter")
                .AddParameter("TenantId", NHibernate.NHibernateUtil.Int32);
        }
    }

    public class DeletedConditionFilter : FilterDefinition
    {
        public DeletedConditionFilter()
        {
            WithName("DeletedConditionFilter")
                .AddParameter("Archived", NHibernate.NHibernateUtil.Boolean);
        }
    }
}