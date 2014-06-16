using MF.Core.Security;

namespace DBFluentMigration.Iteration_1
{
    public class UpdateOperations
    {
        private readonly IOperations _operations;

        public UpdateOperations(IOperations operations)
        {
            _operations = operations;
        }

        public void Update()
        {
            CreateControllerOptions();
            CreateMenuItemOptions();
        }

        public void CreateControllerOptions()
        {
            _operations.CreateOperationForControllerType("TrainerSessionViewController");
            _operations.CreateOperationForControllerType("TrainerSessionVerificationListController");
            _operations.CreateOperationForControllerType("TrainerSessionVerificationController");
            _operations.CreateOperationForControllerType("ActivityController");
            _operations.CreateOperationForControllerType("LocationListController");
            _operations.CreateOperationForControllerType("LocationController");
        }

        public void CreateMenuItemOptions()
        {
            _operations.CreateOperationForMenuItem("SessionReport");
            _operations.CreateOperationForMenuItem("SessionVerification");
            _operations.CreateOperationForMenuItem("SessionVerification/Historical");
            _operations.CreateOperationForMenuItem("SessionVerification/Current");
            _operations.CreateOperationForMenuItem("Reports/Activity");
            _operations.CreateOperationForMenuItem("Loactions");
        }
    }
}