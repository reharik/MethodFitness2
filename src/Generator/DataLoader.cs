using System;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Controllers;
using Rhino.Security.Interfaces;
using StructureMap;

namespace Generator
{
    public class DataLoader
    {
        private IRepository _repository;
        private ISecurityDataService _securityDataService;
        private IDynamicExpressionQuery _dynamicExpressionQuery;
        private User _defaultUser;
        private UserRole _userRoleTrainer;
        private UserRole _userRoleAdmin;
        private User _defaultUser1;
        private Client _client1;
        private Client _client2;
        private Client _client3;
        private Client _client4;
        private Client _client5;
        private IAuthorizationRepository _authorizationRepository;


        public void Load()
        {
            _dynamicExpressionQuery = ObjectFactory.GetInstance<IDynamicExpressionQuery>();
            _repository = ObjectFactory.GetNamedInstance<IRepository>("NoFiltersOrInterceptor");
            _securityDataService = ObjectFactory.GetInstance<ISecurityDataService>();
            _authorizationRepository = ObjectFactory.GetInstance<IAuthorizationRepository>();

            _repository.Initialize();
            createCompany();
            createLocations();
            createUserRoles();
            createUser();
//            CreateClients();
            _repository.Commit();
        }

        private void createCompany()
        {
            var company = new Company{Name = "Method Fitness"};
            _repository.Save(company);
        }

        private void createLocations()
        {
            var location1 = new Location { Name = "West", CompanyId = 1};
            var location2 = new Location { Name = "East", CompanyId = 1 };
            _repository.Save(location1);
            _repository.Save(location2);
        }

        private void createUserRoles()
        {
            _userRoleTrainer = new UserRole
                                   {
                                       Name = "Trainer"
                                   };
            _userRoleAdmin = new UserRole
                                 {
                                     Name = "Administrator"
                                 };
            _repository.Save(_userRoleAdmin);
            _repository.Save(_userRoleTrainer);

        }

        private void createUser()
        {
            var salt = _securityDataService.CreateSalt();
            var passwordHash = _securityDataService.CreatePasswordHash("123", salt);
            _defaultUser = new User
                               {
                                   FirstName = "Raif",
                                   LastName = "Harik",
                                   CompanyId = 1,
                                   Color = "#148509",
                                   CreateDate = DateTime.Now
                               };
            _defaultUser.AddUserRole(_userRoleTrainer);
            _defaultUser.AddUserRole(_userRoleAdmin);
            _defaultUser.UserLoginInfo = new UserLoginInfo
                                             {
                                                 LoginName = "Admin",
                                                 Password = passwordHash,
                                                 Salt = salt,
                                                 CompanyId = 1
                                             };
            var salt1 = _securityDataService.CreateSalt();
            var passwordHash1 = _securityDataService.CreatePasswordHash("123", salt1);
            _defaultUser1 = new User
            {
                FirstName = "Amahl",
                LastName = "Harik",
                CompanyId = 1,
                Color = "#c41d1d",
                CreateDate = DateTime.Now
            };
            _defaultUser1.AddUserRole(_userRoleTrainer);
            _defaultUser1.UserLoginInfo = new UserLoginInfo
            {
                LoginName = "Trainer",
                Password = passwordHash1,
                Salt = salt1,
                CompanyId = 1
            };
            _repository.Save(_defaultUser);
            _repository.Save(_defaultUser1);
        
        }
        public void CreateClients()
        {
            _client1 = new Client
                           {
                               Address1 = "1706 willow st",
                               City = "Austin",
                               State="TX",
                               ZipCode = "78702",
                               Email = "reharik@gmail.com",
                               CompanyId = 1,
                               FirstName = "Raif",
                               LastName = "Harik",
                               MobilePhone = "512.228.6069"
                           };
            _client2 = new Client
            {
                Address1 = "1706 willow st",
                City = "Austin",
                State = "TX",
                ZipCode = "78702",
                Email = "fdsafaas",
                CompanyId = 1,
                FirstName = "Ralf",
                LastName = "harris",
                MobilePhone = "512.228.6069"
            };
            _client3 = new Client
            {
                Address1 = "1706 willow st",
                City = "Austin",
                State = "TX",
                ZipCode = "78702",
                Email = "Green",
                CompanyId = 1,
                FirstName = "Green",
                LastName = "Jarvis",
                MobilePhone = "512.228.6069"
            };
            _client4 = new Client
            {
                Address1 = "1706 willow st",
                City = "Austin",
                State = "TX",
                ZipCode = "78702",
                Email = "Brandon",
                CompanyId = 1,
                FirstName = "Brandon",
                LastName = "Mcclary",
                MobilePhone = "512.228.6069"
            };
            _client5 = new Client
            {
                Address1 = "1706 willow st",
                City = "Austin",
                State = "TX",
                ZipCode = "78702",
                Email = "schmudge",
                CompanyId = 1,
                FirstName = "Schmudge",
                LastName = "Harik",
                MobilePhone = "512.228.6069"
            };
            _repository.Save(_client1);
            _repository.Save(_client2);
            _repository.Save(_client3);
            _repository.Save(_client4);
            _repository.Save(_client5);
        }
    }
}
