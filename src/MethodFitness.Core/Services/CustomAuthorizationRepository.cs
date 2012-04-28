using System.Collections.Generic;
using MethodFitness.Security.Model;
using MethodFitness.Security.Services;
using NHibernate;

namespace MethodFitness.Core.Services
{
    public class CustomAuthorizationRepository : AuthorizationRepository
    {
        private readonly ISession _session;

        public CustomAuthorizationRepository(ISession session)
            : base(session)
        {
            _session = session;
        }

        public virtual IEnumerable<Operation> GetAllOperations()
        {
            return _session.CreateCriteria<Operation>()
                .SetCacheable(true).List<Operation>();
        }

    }
}