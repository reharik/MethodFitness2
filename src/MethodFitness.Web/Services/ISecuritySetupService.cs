using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MethodFitness.Core.Services;
using MethodFitness.Security.Interfaces;
using MethodFitness.Web.Config;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Web.Controllers;
using StructureMap;

namespace MethodFitness.Web.Services
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
//            AssociateAllUsersWithThierTypeGroup();
//            CreateKYTAdminOperation();
//            CreateOperationsForAllControllers();
//            CreateOperationsForAllMenuItems();
//            CreateMiscellaneousOperations();
//           // _permissionsService.GrantDefaultAdminPermissions("Administrator");
//            _permissionsService.GrantDefaultTrainersPermissions();
//            _repository.UnitOfWork.Commit();
        }

        private void CreateKYTAdminOperation()
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
            menuConfig.Each(x =>
            {
                var menuItems = x.Build(true);
                menuItems.Each(m =>
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

           createOperation("/TrainerPayment/Display");
           createOperation("/TrainerPayment/Active");
           createOperation("/Payment/Display");
           createOperation("/Payment/AddUpdate");
           createOperation("/Billing/ChangeClientRates");

        }

        public void AssociateAllUsersWithThierTypeGroup()
        {
            var admins = _repository.Query<User>(x => x.UserRoles.Any(y=>y.Name == SecurityUserGroups.Administrator.ToString()));
            admins.Each(x =>
                _authorizationRepository.AssociateUserWith(x, SecurityUserGroups.Administrator.ToString()));
            var employees = _repository.FindAll<User>();
            employees.Each(x => _authorizationRepository.AssociateUserWith(x, SecurityUserGroups.Trainer.ToString()));
        }

        public void CreateUserGroups()
        {
            if (_authorizationRepository.GetUsersGroupByName(SecurityUserGroups.Administrator.ToString()) == null)
            {
                _authorizationRepository.CreateUsersGroup(SecurityUserGroups.Administrator.ToString());
            }
            if (_authorizationRepository.GetUsersGroupByName(SecurityUserGroups.Trainer.ToString()) == null)
            {
                _authorizationRepository.CreateUsersGroup(SecurityUserGroups.Trainer.ToString());
            }
        }

       

    }
}