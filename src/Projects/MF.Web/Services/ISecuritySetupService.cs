using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CC.Core.Core.DomainTools;
using CC.Core.Security.Interfaces;
using CC.Core.Utilities;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Controllers;
using MF.Web.Menus;
using StructureMap;

namespace MF.Web.Services
{
    public interface ISecuritySetupService
    {
        void ExecuteAll();
        void CreateOperationsForAllMenuItems();
        void AssociateAllUsersWithThierTypeGroup();
    }

    public class DefaultSecuritySetupService : ISecuritySetupService
    {
        private static readonly List<string> Operations = new List<string>();
        private readonly IContainer _container;
        private readonly IRepository _repository;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IPermissionsBuilderService _permissionsBuilderService;
        private readonly IMFPermissionsService _permissionsService;

        public DefaultSecuritySetupService(IContainer container,
            IAuthorizationRepository authorizationRepository,
            IPermissionsBuilderService permissionsBuilderService,
            IMFPermissionsService permissionsService,
            IRepository repository)
        {
            _container = container;
            //_repository = ObjectFactory.Container.GetInstance<IRepository>("NoFilters");
            _authorizationRepository = authorizationRepository;
            _permissionsBuilderService = permissionsBuilderService;
            _permissionsService = permissionsService;
            _repository = repository;
        }

        public void ExecuteAll()
        {
            CreateUserGroups();
            AssociateAllUsersWithThierTypeGroup();
            CreateMFAdminOperation();
            CreateOperationsForAllControllers();
            CreateOperationsForAllMenuItems();
            CreateMiscellaneousOperations();
            _permissionsService.GrantDefaultAdminPermissions("Administrator");
            _permissionsService.GrantDefaultAdminPermissions("Manager");
            _permissionsService.GrantDefaultTrainersPermissions();
            _repository.UnitOfWork.Commit();
        }

        private void CreateMFAdminOperation()
        {
            createOperation("/AdminOrGreater");
            createOperation("/MFAdmin");
        }

        private void CreateOperationsForAllControllers()
        {
            foreach (Type controllerType in typeof(MFController)
                .Assembly
                .GetTypes()
                .Where(x => (typeof(Controller).IsAssignableFrom(x)) && !x.IsAbstract))
            {
                //foreach ( var method in controllerType
                //    .GetMethods()
                //    .Where(x => (typeof (ActionResult).IsAssignableFrom(x.ReturnType)) && !x.IsDefined(typeof (NonActionAttribute), true)))
                //{
                var operation = string.Format("/{0}", controllerType.Name);
                if (!Operations.Contains(operation))
                {
                    Operations.Add(operation);
                    createOperation(operation);
                }
                //}
            }
        }
        private void createOperation(string operation)
        {
            if (_authorizationRepository.GetOperationByName(operation) == null)
                _authorizationRepository.CreateOperation(operation);
        }

        public void CreateOperationsForAllMenuItems()
        {
            var menuConfig = _container.GetAllInstances<IMenuConfig>();
            menuConfig.ForEachItem(x =>
            {
                var menuItems = x.Build(true);
                menuItems.ForEachItem(m =>
                {
                    var operation = "/MenuItem/" + m.Text.RemoveWhiteSpace();
                    if (!Operations.Contains(operation))
                    {
                        Operations.Add(operation);
                        createOperation(operation);
                    }
                });
            });
        }

        public void CreateMiscellaneousOperations()
        {
           createOperation("/Calendar/CanSeeOthersAppointments");
           createOperation("/Calendar/CanEditOtherAppointments");
           createOperation("/Calendar/CanEnterRetroactiveAppointments");
           createOperation("/Calendar/CanEditPastAppointments");
           createOperation("/Calendar/SetAppointmentForOthers");
           createOperation("/Calendar/CanDeleteRetroactiveAppointments");
           createOperation("/Clients/CanScheduleAllClients");
           createOperation("/Clients/CanDeleteClients");

           createOperation("/TrainerPayment/Display");
           createOperation("/TrainerPayment/Active");
           createOperation("/Payment/Display");
           createOperation("/Payment/AddUpdate");
           createOperation("/Billing/ChangeClientRates");

        }

        public void AssociateAllUsersWithThierTypeGroup()
        {
           // _repository.DisableFilter("CompanyConditionFilter");
            var admins = _repository.Query<User>(x => x.UserRoles.Any(y => y.Name == UserType.Administrator.ToString()));
            admins.ForEachItem(x =>
                _authorizationRepository.AssociateUserWith(x, UserType.Administrator.ToString()));
            var employees = _repository.FindAll<User>();
            employees.ForEachItem(x => _authorizationRepository.AssociateUserWith(x, UserType.Trainer.ToString()));
            var manager = _repository.FindAll<User>();
            employees.ForEachItem(x => _authorizationRepository.AssociateUserWith(x, UserType.Manager.ToString()));
        }

        public void CreateUserGroups()
        {
            if (_authorizationRepository.GetUsersGroupByName(UserType.Administrator.ToString()) == null)
            {
                _authorizationRepository.CreateUsersGroup(UserType.Administrator.ToString());
            }
            if (_authorizationRepository.GetUsersGroupByName(UserType.Trainer.ToString()) == null)
            {
                _authorizationRepository.CreateUsersGroup(UserType.Trainer.ToString());
            }
            if (_authorizationRepository.GetUsersGroupByName(UserType.Manager.ToString()) == null)
            {
                _authorizationRepository.CreateUsersGroup(UserType.Manager.ToString());
            }
        }

       

    }
}