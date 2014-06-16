
using MF.Core.Security;

namespace DBFluentMigration.Iteration_0
{
    public class CreateInitialOperations
    {
        private readonly IOperations _operations;

        public CreateInitialOperations(IOperations operations)
        {
            _operations = operations;
        }

        public void Update()
        {
            CreateControllerOptions();
            CreateMenuItemOptions();
            CreateMiscItems();
        }
        public void CreateControllerOptions()
        {
            _operations.CreateOperationForControllerType("ClientController");
            _operations.CreateOperationForControllerType("ClientListController");
            _operations.CreateOperationForControllerType("EmployeeDashboardController");
            _operations.CreateOperationForControllerType("MethodFitnessController");
            _operations.CreateOperationForControllerType("MFController");
            _operations.CreateOperationForControllerType("OrthoganalController");
            _operations.CreateOperationForControllerType("ScheduledTasksController");
            _operations.CreateOperationForControllerType("TrainerController");
            _operations.CreateOperationForControllerType("TrainerListController");
            _operations.CreateOperationForControllerType("AppointmentCalendarController");
            _operations.CreateOperationForControllerType("AppointmentController");
            _operations.CreateOperationForControllerType("TimeSheetController");
            _operations.CreateOperationForControllerType("PaymentController");
            _operations.CreateOperationForControllerType("PaymentListController");
            _operations.CreateOperationForControllerType("PayTrainerController");
            _operations.CreateOperationForControllerType("PayTrainerListController");
            _operations.CreateOperationForControllerType("TrainerPaymentController");
            _operations.CreateOperationForControllerType("TrainerPaymentListController");
        }

        public void CreateMenuItemOptions()
        {
            _operations.CreateOperationForMenuItem("Calendar");
            _operations.CreateOperationForMenuItem("Clients");
            _operations.CreateOperationForMenuItem("AdminTools");
            _operations.CreateOperationForMenuItem("Trainers");
        }

        public void CreateMiscItems()
        {
            _operations.CreateOperation("/Calendar/CanSeeOthersAppointments");
            _operations.CreateOperation("/Calendar/CanEditOtherAppointments");
            _operations.CreateOperation("/Calendar/CanEnterRetroactiveAppointments");
            _operations.CreateOperation("/Calendar/CanEditPastAppointments");
            _operations.CreateOperation("/Calendar/SetAppointmentForOthers");
            _operations.CreateOperation("/Calendar/CanDeleteRetroactiveAppointments");
            _operations.CreateOperation("/Clients/CanScheduleAllClients");
            _operations.CreateOperation("/Clients/CanDeleteClients");
            _operations.CreateOperation("/TrainerPayment/Display");
            _operations.CreateOperation("/TrainerPayment/Active");
            _operations.CreateOperation("/Payment/Display");
            _operations.CreateOperation("/Payment/AddUpdate");
            _operations.CreateOperation("/Billing/ChangeClientRates");
        }
    }
}