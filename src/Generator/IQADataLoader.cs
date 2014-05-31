using System;
using CC.Core.DomainTools;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Controllers;

namespace Generator
{
    public interface IQADataLoader
    {
        void Load();
    }

    public class QADataLoader : IQADataLoader
    {
        private readonly IRepository _repository;
        private readonly ISecurityDataService _securityDataService;
        private User _admin1;
        private User _admin2;
        private BaseSessionRate _baseSessionRate;

        public QADataLoader(IRepository repository,
            ISecurityDataService securityDataService)
        {
            _repository = repository;
            _securityDataService = securityDataService;
        }

        public void Load()
        {
            createCompany();
            createUser();
            createLocations();
            createBaseSessionRates();
            CreateClients();
            _repository.Commit();
        }

        private void createBaseSessionRates()
        {
            _baseSessionRate = new BaseSessionRate
                {
                    FullHour = 65,
                    HalfHour = 40,
                    FullHourTenPack = 600,
                    HalfHourTenPack = 350,
                    Pair = 45,
                    PairTenPack = 400
                };
            _repository.Save(_baseSessionRate);
        }


        private void createCompany()
        {
            var company = new Company { Name = "Method Fitness" };
            _repository.Save(company);
        }

        private void createLocations()
        {
            var location1 = new Location { Name = "West", CompanyId = 1 };
            var location2 = new Location { Name = "East", CompanyId = 1 };
            _repository.Save(location1);
            _repository.Save(location2);
        }

        private void createUser()
        {
            var userRoleTrainer = new UserRole
            {
                Name = "Trainer"
            };
            var userRoleAdmin = new UserRole
            {
                Name = "Administrator"
            };

            var salt = _securityDataService.CreateSalt();
            var passwordHash = _securityDataService.CreatePasswordHash("123", salt);
            _admin1 = new User
            {
                FirstName = "Admin",
                LastName = "Admin",
                CompanyId = 1,
                CreatedDate = DateTime.Now,
                PhoneMobile = "401.743.9669",
                Email = "methodfit@gmail.com",
                ClientRateDefault = 65
            };
            _admin1.AddUserRole(userRoleTrainer);
            _admin1.AddUserRole(userRoleAdmin);
            _admin1.UserLoginInfo = new UserLoginInfo
            {
                LoginName = "Admin",
                Password = passwordHash,
                Salt = salt,
                CompanyId = 1
            };
            var salt1 = _securityDataService.CreateSalt();
            var passwordHash1 = _securityDataService.CreatePasswordHash("123", salt1);
            _admin2 = new User()
            {
                FirstName = "Amahl",
                LastName = "Harik",
                CompanyId = 1,
                Color = "#148509",
                ClientRateDefault = 65,
                CreatedDate = DateTime.Now,
                PhoneMobile = "123456789",
                Email = "methodfit@gmail.com"
            };
            _admin2.AddUserRole(userRoleTrainer);
            _admin2.UserLoginInfo = new UserLoginInfo
            {
                LoginName = "aih",
                Password = passwordHash1,
                Salt = salt1,
                CompanyId = 1
            };
            _repository.Save(_admin1);
            _repository.Save(_admin2);

        }

        public void CreateClients()
        {
                        var client1 = new Client
                                       {
                                           Address1 = "1706 willow st",
                                           City = "Austin",
                                           State="TX",
                                           ZipCode = "78702",
                                           Email = "reharik@gmail.com",
                                           CompanyId = 1,
                                           FirstName = "Raif",
                                           LastName = "Harik",
                                           MobilePhone = "512.228.6069",
                                           StartDate = DateTime.Now,
                                           CreatedDate = DateTime.Now,
                                           SessionRates = new SessionRates(_baseSessionRate)
                                       };
                        var client2 = new Client
                        {
                            Address1 = "1706 willow st",
                            City = "Austin",
                            State = "TX",
                            ZipCode = "78702",
                            Email = "fdsafaas",
                            CompanyId = 1,
                            FirstName = "Ralf",
                            LastName = "harris",
                            MobilePhone = "512.228.6069",
                            StartDate = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            SessionRates = new SessionRates(_baseSessionRate)
                        };
                        var client3 = new Client
                        {
                            Address1 = "1706 willow st",
                            City = "Austin",
                            State = "TX",
                            ZipCode = "78702",
                            Email = "Green",
                            CompanyId = 1,
                            FirstName = "Green",
                            LastName = "Jarvis",
                            MobilePhone = "512.228.6069",
                            StartDate = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            SessionRates = new SessionRates(_baseSessionRate)
                        };
                        var client4 = new Client
                        {
                            Address1 = "1706 willow st",
                            City = "Austin",
                            State = "TX",
                            ZipCode = "78702",
                            Email = "Brandon",
                            CompanyId = 1,
                            FirstName = "Brandon",
                            LastName = "Mcclary",
                            MobilePhone = "512.228.6069",
                            StartDate = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            SessionRates = new SessionRates(_baseSessionRate)
                        };
                        var client5 = new Client
                        {
                            Address1 = "1706 willow st",
                            City = "Austin",
                            State = "TX",
                            ZipCode = "78702",
                            Email = "schmudge",
                            CompanyId = 1,
                            FirstName = "Schmudge",
                            LastName = "Harik",
                            MobilePhone = "512.228.6069",
                            StartDate = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            SessionRates = new SessionRates(_baseSessionRate)
                        };
                        _admin1.AddClient(client1, _admin1.ClientRateDefault);
                        _admin1.AddClient(client2, _admin1.ClientRateDefault);
                        _admin1.AddClient(client3, _admin1.ClientRateDefault);
                        _admin1.AddClient(client4, _admin1.ClientRateDefault);
                        _admin1.AddClient(client5, _admin1.ClientRateDefault);
                        _repository.Save(_admin1);
        }
    
    }
}
