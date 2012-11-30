using CC.Security.Interfaces;
using KnowYourTurf.Web.Security;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;

namespace DBFluentMigration.Iteration_0
{
    public class CreateInitialPermissions
    {
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IPermissions _permissions;

        public CreateInitialPermissions(IAuthorizationRepository authorizationRepository, 
            IPermissions permissions)
        {
            _authorizationRepository = authorizationRepository;
            _permissions = permissions;
        }

        public void Update()
        {
            GrantAdminPermissions();
            GrantTrainerPermissions();
        }

        public void GrantAdminPermissions()
        {
            var operations = ((CustomAuthorizationRepository)_authorizationRepository).GetAllOperations();
            foreach (var operation in operations)
            {
                _permissions.CreatePermission(UserType.Administrator, operation.Name, 10);
            }
        }

        
        public void GrantTrainerPermissions()
        {
            _permissions.CreateControllerPermission("KnowYourTurfController", UserType.Trainer);
            _permissions.CreateControllerPermission("OrthogonalController", UserType.Trainer);
            _permissions.CreateControllerPermission("CalculatorController", UserType.Trainer);
            _permissions.CreateControllerPermission("CalculatorListController", UserType.Trainer);
            _permissions.CreateControllerPermission("TrainerDashboardController", UserType.Trainer);
            _permissions.CreateControllerPermission("TrainerController", UserType.Trainer);
            _permissions.CreateControllerPermission("EquipmentController", UserType.Trainer);
            _permissions.CreateControllerPermission("EquipmentListController", UserType.Trainer);
            _permissions.CreateControllerPermission("EventController", UserType.Trainer);
            _permissions.CreateControllerPermission("EventCalendarController", UserType.Trainer);
            _permissions.CreateControllerPermission("FieldController", UserType.Trainer);
            _permissions.CreateControllerPermission("FieldDashboardController", UserType.Trainer);
            _permissions.CreateControllerPermission("FieldListController", UserType.Trainer);
            _permissions.CreateControllerPermission("ForumController", UserType.Trainer);
            _permissions.CreateControllerPermission("InventoryListController", UserType.Trainer);
            _permissions.CreateControllerPermission("PhotoController", UserType.Trainer);
            _permissions.CreateControllerPermission("DocumentController", UserType.Trainer);
            _permissions.CreateControllerPermission("PurchaseOrderCommitController", UserType.Trainer);
            _permissions.CreateControllerPermission("PurchaseOrderController", UserType.Trainer);
            _permissions.CreateControllerPermission("PurchaseOrderLineItemController", UserType.Trainer);
            _permissions.CreateControllerPermission("PurchaseOrderListController", UserType.Trainer);
            _permissions.CreateControllerPermission("TaskController", UserType.Trainer);
            _permissions.CreateControllerPermission("TaskCalendarController", UserType.Trainer);
            _permissions.CreateControllerPermission("TaskListController", UserType.Trainer);
            _permissions.CreateControllerPermission("VendorContactController", UserType.Trainer);
            _permissions.CreateControllerPermission("VendorContactListController", UserType.Trainer);
            _permissions.CreateControllerPermission("VendorController", UserType.Trainer);
            _permissions.CreateControllerPermission("VendorListController", UserType.Trainer);
            _permissions.CreateControllerPermission("WeatherController", UserType.Trainer);
            _permissions.CreateControllerPermission("WeatherListController", UserType.Trainer);
            _permissions.CreateControllerPermission("EquipmentDashboardController", UserType.Trainer);
            _permissions.CreateControllerPermission("EquipmentTaskCalendarController", UserType.Trainer);
            _permissions.CreateControllerPermission("EquipmentTaskController", UserType.Trainer);
            _permissions.CreateControllerPermission("EquipmentTaskListController", UserType.Trainer);

            _permissions.CreateMenuPermission(UserType.Trainer, "EquipmentTasks");
            _permissions.CreateMenuPermission(UserType.Trainer, "EquipmentTasksLists");
            _permissions.CreateMenuPermission(UserType.Trainer, "EquipmentTasks");
            _permissions.CreateMenuPermission(UserType.Trainer, "CompletedEquipmentTasks");
            _permissions.CreateMenuPermission(UserType.Trainer, "EquipmentTaskCalendar");
            _permissions.CreateMenuPermission(UserType.Trainer, "Home");
            _permissions.CreateMenuPermission(UserType.Trainer, "Fields");
            _permissions.CreateMenuPermission(UserType.Trainer, "Equipment");
            _permissions.CreateMenuPermission(UserType.Trainer, "Tasks");
            _permissions.CreateMenuPermission(UserType.Trainer, "Events");
            _permissions.CreateMenuPermission(UserType.Trainer, "Calculators");
            _permissions.CreateMenuPermission(UserType.Trainer, "Weather");
            _permissions.CreateMenuPermission(UserType.Trainer, "Forum");
        }

        
    }
}