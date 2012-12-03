using KnowYourTurf.Web.Security;
using MethodFitness.Core;
using MethodFitness.Core.Enumerations;

namespace DBFluentMigration.Iteration_1
{
    public class UpdatePermissions
    {
        private readonly IPermissions _permissions;

        public UpdatePermissions(IPermissions permissions)
        {
            _permissions = permissions;
        }

        public void Update()
        {
            GrantAdminPermissions();
            GrantTrainerPermissions();
        }

        private void GrantTrainerPermissions()
        {
            throw new System.NotImplementedException();
        }

        public void GrantAdminPermissions()
        {
            _permissions.CreateControllerPermission(UserType.Administrator, "ClientController");
            _permissions.CreateControllerPermission(UserType.Administrator, "ClientListController");
            _permissions.CreateControllerPermission(UserType.Administrator, "EmployeeDashboardController");
        }
    }
}