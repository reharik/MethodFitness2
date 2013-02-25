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
            GrantTrainerPermissions();
            GrantAdminPermissions();
        }

        private void GrantAdminPermissions()
        {
            _permissions.CreateControllerPermission(UserType.Administrator, "ActivityController");
            _permissions.CreateMenuPermission(UserType.Administrator, "Reports/Activity");
        }

        private void GrantTrainerPermissions()
        {
            _permissions.CreateControllerPermission(UserType.Trainer, "TrainerSessionViewController");
            _permissions.CreateControllerPermission(UserType.Trainer, "TrainerSessionVerificationListController");
            _permissions.CreateControllerPermission(UserType.Trainer, "TrainerSessionVerificationController");
            _permissions.CreateMenuPermission(UserType.Trainer, "SessionReport");
            _permissions.CreateMenuPermission(UserType.Trainer, "SessionVerification");
            _permissions.CreateMenuPermission(UserType.Trainer, "SessionVerification/Historical");
            _permissions.CreateMenuPermission(UserType.Trainer, "SessionVerification/Current");
        }

    }
}