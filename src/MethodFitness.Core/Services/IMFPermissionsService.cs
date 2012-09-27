using CC.Security.Interfaces;

namespace MethodFitness.Core.Services
{
    public interface IMFPermissionsService
    {
        void GrantDefaultAdminPermissions(string type);
        void GrantDefaultTrainersPermissions();
    }

    public class MFPermissionsService : IMFPermissionsService
    {
        private readonly IPermissionsBuilderService _permissionsBuilderService;
        private readonly IAuthorizationRepository _authorizationRepository;

        public MFPermissionsService(IPermissionsBuilderService permissionsBuilderService, IAuthorizationRepository authorizationRepository)
        {
            _permissionsBuilderService = permissionsBuilderService;
            _authorizationRepository = authorizationRepository;
        }


        public void GrantDefaultAdminPermissions(string type)
        {
            var operations = ((CustomAuthorizationRepository)_authorizationRepository).GetAllOperations();
            foreach (var operation in operations)
            {
                //this is a fucking hack obviously
                if (operation.Name != "/MFAdmin" && operation.Name != "/Payment/Display" & operation.Name != "/TrainerPayment/Display")
                    _permissionsBuilderService
                        .Allow(operation)
                        .For(type)
                        .OnEverything()
                        .Level(10)
                        .Save();
            }
            _permissionsBuilderService.Deny("/Payment/Display").For(type).OnEverything().Level(11).Save();
        }

        public void GrantDefaultTrainersPermissions()
        {
            _permissionsBuilderService.Allow("/AppointmentCalendarController").For("Trainer").OnEverything().Level(1).Save();
            _permissionsBuilderService.Allow("/AppointmentController").For("Trainer").OnEverything().Level(1).Save();

            _permissionsBuilderService.Allow("/MenuItem/Calendar").For("Trainer").OnEverything().Level(1).Save();
            _permissionsBuilderService.Allow("/MenuItem/Clients").For("Trainer").OnEverything().Level(1).Save();

            _permissionsBuilderService.Allow("/Payment/Display").For("Trainer").OnEverything().Level(1).Save();
            _permissionsBuilderService.Allow("/TrainerPayment/Display").For("Trainer").OnEverything().Level(1).Save();
        }

    }
}