using System.Linq;
using CC.Core.Core.DomainTools;
using CC.Core.Security.Interfaces;
using CC.Core.Utilities;
using MF.Core.Domain;
using MF.Core.Enumerations;

namespace MF.Core.Security
{
    public interface IUserGroups
    {
        void CreateUserGroups();
        void RemoveUserGroups(SecurityUserGroups group);
        void AssociateAllUsersWithTheirTypeGroup();
        void DissassociateUsersWithTheirGroup(SecurityUserGroups group);
    }

    public class UserGroups : IUserGroups
    {
        private readonly IRepository _repository;
        private readonly IAuthorizationRepository _authorizationRepository;

        public UserGroups(IRepository repository, IAuthorizationRepository authorizationRepository)
        {
            _repository = repository;
            _authorizationRepository = authorizationRepository;
        }

        public void CreateUserGroups()
        {
            _authorizationRepository.CreateUsersGroup(SecurityUserGroups.Administrator.ToString());
            _authorizationRepository.CreateUsersGroup(SecurityUserGroups.Trainer.ToString());
        }

        public void RemoveUserGroups(SecurityUserGroups group)
        {
            _authorizationRepository.RemoveUsersGroup(group.Key);
        }

        public void AssociateAllUsersWithTheirTypeGroup()
        {
            var admins = _repository.Query<User>(x => x.UserRoles.Any(y => y.Role.Name == SecurityUserGroups.Administrator.ToString()));
            admins.ForEachItem(x => _authorizationRepository.AssociateUserWith(x, SecurityUserGroups.Administrator.ToString()));
            var employees = _repository.Query<User>(x => x.UserRoles.Any(y => y.Role.Name == SecurityUserGroups.Trainer.ToString()));
            employees.ForEachItem(x => _authorizationRepository.AssociateUserWith(x, SecurityUserGroups.Trainer.ToString()));
        }

        public void DissassociateUsersWithTheirGroup(SecurityUserGroups group)
        {
            var users = _repository.Query<User>(x => x.UserRoles.Any(y => y.Role.Name == group.Key));
            users.ForEachItem(x => _authorizationRepository.DetachUserFromGroup(x, group.Key));
        }
    }
}