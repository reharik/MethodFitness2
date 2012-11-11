using CC.Core.DomainTools;
using CC.Security.Interfaces;
using MethodFitness.Core.Domain;
using StructureMap;

namespace MethodFitness.Core.Services
{
    public interface IUserPermissionService
    {
        bool IsAllowed(string operationName);
    }

    public class UserPermissionService : IUserPermissionService
    {
        private readonly ISessionContext _sessionContext;
        private readonly IRepository _repository;
        private readonly IAuthorizationService _authorizationService;

        public UserPermissionService(ISessionContext sessionContext, IRepository repository, IAuthorizationService authorizationService)
        {
            _sessionContext = sessionContext;
            _repository = repository;
            _authorizationService = authorizationService;
        }

        public bool IsAllowed(string operationName)
        {
            var userEntityId = _sessionContext.GetUserId();
            var user = _repository.Find<User>(userEntityId);
            return _authorizationService.IsAllowed(user, operationName);
        }

        public static bool Allow(string operationName)
        {
            var sessionContext = ObjectFactory.Container.GetInstance<SessionContext>();
            var userEntityId = sessionContext.GetUserId();
            var repository = ObjectFactory.Container.GetInstance<IRepository>();
            var user = repository.Find<User>(userEntityId);
            var authorizationService = ObjectFactory.Container.GetInstance<IAuthorizationService>();
            return authorizationService.IsAllowed(user, operationName);
        }
    }
}