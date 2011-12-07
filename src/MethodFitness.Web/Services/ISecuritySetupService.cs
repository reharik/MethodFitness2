using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KnowYourTurf.Core.Services;
using KnowYourTurf.Web.Config;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Web.Controllers;
using Rhino.Security.Interfaces;
using StructureMap;

namespace MethodFitness.Web.Services
{
    public interface ISecuritySetupService
    {
        void ExecuteAll();
        void CreateOperationsForAllMenuItems();
        void AssociateAllUsersWithThierTypeGroup();
        void CreateUserGroups();
        void CreateAdminPermissions();
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
            IMFPermissionsService permissionsService)
        {
            _container = container;
            _repository = new Repository();
            _authorizationRepository = authorizationRepository;
            _permissionsBuilderService = permissionsBuilderService;
            _permissionsService = permissionsService;
        }

        public void ExecuteAll()
        {
            CreateUserGroups();
            AssociateAllUsersWithThierTypeGroup();
            CreateKYTAdminOperation();
            CreateOperationsForAllControllers();
            CreateOperationsForAllMenuItems();
            CreateAdminPermissions();
            _permissionsService.GrantDefaultAdminPermissions("Administrator");
            _permissionsService.GrantDefaultTrainersPermissions();
            CreateFacilitiesPermissions();
            _repository.UnitOfWork.Commit();
        }

        private void CreateKYTAdminOperation()
        {
            _authorizationRepository.CreateOperation("/AdminOrGreater");
            _authorizationRepository.CreateOperation("/KYTAdmin");
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
                    if (_authorizationRepository.GetOperationByName(operation) == null)
                        _authorizationRepository.CreateOperation(operation);
                }
                //}
            }
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
                        if (_authorizationRepository.GetOperationByName(operation) == null)
                            _authorizationRepository.CreateOperation(operation);
                    }
                });
            });
        }

        public void CreateMiscellaneousOperations()
        {
            _authorizationRepository.CreateOperation("/Calendar/CanSeeOtherAppointments");
            _authorizationRepository.CreateOperation("/Calendar/CanEditOtherAppointments");
            _authorizationRepository.CreateOperation("/Calendar/CanEnterRetroactiveAppointments");
            _authorizationRepository.CreateOperation("/Calendar/CanEditPastAppointments");
        }

        public void AssociateAllUsersWithThierTypeGroup()
        {
            var admins = _repository.Query<User>(x => x.UserRoles.Any(y=>y.Name == "Administrator"));
            admins.Each(x => _authorizationRepository.AssociateUserWith(x, "Administrator"));
            var employees = _repository.FindAll<User>();
            employees.Each(x => _authorizationRepository.AssociateUserWith(x, "Trainer"));
        }

        public void CreateUserGroups()
        {
            if (_authorizationRepository.GetUsersGroupByName("Administrator") == null)
            {
                _authorizationRepository.CreateUsersGroup("Administrator");
            }
            if (_authorizationRepository.GetUsersGroupByName("Trainer") == null)
            {
                _authorizationRepository.CreateUsersGroup("Trainer");
            }
        }

        public void CreateAdminPermissions()
        {

        }

        public void CreateFacilitiesPermissions()
        {


        }
    }
}