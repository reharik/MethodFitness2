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
       

        public void Load(string extraDataKey)
        {
            _dynamicExpressionQuery = ObjectFactory.GetInstance<IDynamicExpressionQuery>();
            _repository = ObjectFactory.GetNamedInstance<IRepository>("NoFiltersOrInterceptor");

            _securityDataService = ObjectFactory.GetInstance<ISecurityDataService>();

            _repository.Initialize();
           
        }

       

    }
}
