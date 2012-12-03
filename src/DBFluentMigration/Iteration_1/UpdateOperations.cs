using MethodFitness.Core;

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
            _operations.CreateOperationForControllerType("TrainerSessionVerificationController");
        }

        public void CreateMenuItemOptions()
        {
            _operations.CreateOperationForMenuItem("SessionReport");
        }
    }
}