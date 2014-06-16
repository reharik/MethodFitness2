using MF.Core.Enumerations;

namespace DBFluentMigration.Iteration_0
{
    public class CreateInitialPermissions
    {
        private readonly IPermissions _permissions;

        public CreateInitialPermissions(IPermissions permissions)
        {
            _permissions = permissions;
        }

        public void Update()
        {
            GrantAdminPermissions();
            GrantTrainerPermissions();
        }

        public void GrantAdminPermissions()
        {
            _permissions.CreateControllerPermission(UserType.Administrator, "ClientController");
            _permissions.CreateControllerPermission(UserType.Administrator, "ClientListController");
            _permissions.CreateControllerPermission(UserType.Administrator, "EmployeeDashboardController");
            _permissions.CreateControllerPermission(UserType.Administrator, "MethodFitnessController");
            _permissions.CreateControllerPermission(UserType.Administrator, "MFController");
            _permissions.CreateControllerPermission(UserType.Administrator, "OrthoganalController");
            _permissions.CreateControllerPermission(UserType.Administrator, "ScheduledTasksController");
            _permissions.CreateControllerPermission(UserType.Administrator, "TrainerController");
            _permissions.CreateControllerPermission(UserType.Administrator, "TrainerListController");
            _permissions.CreateControllerPermission(UserType.Administrator, "AppointmentCalendarController");
            _permissions.CreateControllerPermission(UserType.Administrator, "AppointmentController");
            _permissions.CreateControllerPermission(UserType.Administrator, "TimeSheetController");
            _permissions.CreateControllerPermission(UserType.Administrator, "PaymentController");
            _permissions.CreateControllerPermission(UserType.Administrator, "PaymentListController");
            _permissions.CreateControllerPermission(UserType.Administrator, "PayTrainerController");
            _permissions.CreateControllerPermission(UserType.Administrator, "PayTrainerListController");
            _permissions.CreateControllerPermission(UserType.Administrator, "TrainerPaymentController");
            _permissions.CreateControllerPermission(UserType.Administrator, "TrainerPaymentListController");

            _permissions.CreateMenuPermission(UserType.Administrator, "Calendar");
            _permissions.CreateMenuPermission(UserType.Administrator, "Clients");
            _permissions.CreateMenuPermission(UserType.Administrator, "AdminTools");
            _permissions.CreateMenuPermission(UserType.Administrator, "Trainers");

            _permissions.CreatePermission(UserType.Administrator, "/Calendar/CanSeeOthersAppointments");
            _permissions.CreatePermission(UserType.Administrator, "/Calendar/CanEditOtherAppointments");
            _permissions.CreatePermission(UserType.Administrator, "/Calendar/CanEnterRetroactiveAppointments");
            _permissions.CreatePermission(UserType.Administrator, "/Calendar/CanEditPastAppointments");
            _permissions.CreatePermission(UserType.Administrator, "/Calendar/SetAppointmentForOthers");
            _permissions.CreatePermission(UserType.Administrator, "/Calendar/CanDeleteRetroactiveAppointments");
            _permissions.CreatePermission(UserType.Administrator, "/Clients/CanScheduleAllClients");
            _permissions.CreatePermission(UserType.Administrator, "/Clients/CanDeleteClients");
            _permissions.CreatePermission(UserType.Administrator, "/TrainerPayment/Display");
            _permissions.CreatePermission(UserType.Administrator, "/TrainerPayment/Active");
            _permissions.CreatePermission(UserType.Administrator, "/Payment/AddUpdate");
            _permissions.CreatePermission(UserType.Administrator, "/Billing/ChangeClientRates");

        }

        
        public void GrantTrainerPermissions()
        {
            _permissions.CreateControllerPermission(UserType.Trainer, "ClientController");
            _permissions.CreateControllerPermission(UserType.Trainer, "ClientListController");
            _permissions.CreateControllerPermission(UserType.Trainer, "EmployeeDashboardController");
            _permissions.CreateControllerPermission(UserType.Trainer, "MethodFitnessController");
            _permissions.CreateControllerPermission(UserType.Trainer, "MFController");
            _permissions.CreateControllerPermission(UserType.Trainer, "OrthoganalController");
            _permissions.CreateControllerPermission(UserType.Trainer, "ScheduledTasksController");
            _permissions.CreateControllerPermission(UserType.Trainer, "AppointmentCalendarController");
            _permissions.CreateControllerPermission(UserType.Trainer, "AppointmentController");
            _permissions.CreateControllerPermission(UserType.Trainer, "PaymentController");
            _permissions.CreateControllerPermission(UserType.Trainer, "PaymentListController");

            _permissions.CreateMenuPermission(UserType.Trainer, "Calendar");
            _permissions.CreateMenuPermission(UserType.Trainer, "Clients");
            
            _permissions.CreatePermission(UserType.Trainer, "/TrainerPayment/Display");
            _permissions.CreatePermission(UserType.Trainer, "/Payment/Display");

        }
        
    }
}