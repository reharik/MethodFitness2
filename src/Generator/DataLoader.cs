using System;
using System.Collections.Generic;
using System.Linq;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Localization;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;
using MethodFitness.Web.Services;
using StructureMap;

namespace Generator
{
    public class DataLoader
    {
        private IRepository _repository;
        private ISecurityDataService _securityDataService;
        private IDynamicExpressionQuery _dynamicExpressionQuery;
        private User _defaultUser;


        public void Load(string extraDataKey)
        {
            _dynamicExpressionQuery = ObjectFactory.GetInstance<IDynamicExpressionQuery>();
            _repository = ObjectFactory.GetNamedInstance<IRepository>("NoFiltersOrInterceptor");

            _securityDataService = ObjectFactory.GetInstance<ISecurityDataService>();

            _repository.Initialize();

            createUser();
            _repository.Commit();
        }

        private void createUser()
        {
            var salt = _securityDataService.CreateSalt();
            var passwordHash = _securityDataService.CreatePasswordHash("123", salt);
            _defaultUser = new User
                               {
                                   FirstName = "Raif",
                                   LastName = "Harik"
                               };
            _defaultUser.UserLoginInfo = new UserLoginInfo
                                             {
                                                 LoginName = "Admin",
                                                 Password = passwordHash,
                                                 Salt = salt
                                             };
            _repository.Save(_defaultUser);
        }
    }
}
