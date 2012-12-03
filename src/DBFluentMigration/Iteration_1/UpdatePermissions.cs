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
        }

        private void GrantTrainerPermissions()
        {
            _permissions.CreateControllerPermission(UserType.Trainer, "TrainerSessionVerificationController");
            _permissions.CreateMenuPermission(UserType.Trainer, "SessionReport");
        }

    }
}