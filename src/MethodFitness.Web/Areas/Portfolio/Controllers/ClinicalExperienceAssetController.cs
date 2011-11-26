using System.Web.Mvc;
using DecisionCritical.Core;
using DecisionCritical.Core.Domain;
using DecisionCritical.Core.Services;
using DecisionCritical.Web.Controllers;
using DecisionCritical.Web.Models;
using DecisionCritical.Web.Services;

namespace DecisionCritical.Web.Areas.Portfolio.Controllers
{
    public class ClinicalExperienceAssetController :DCIController
    {
        public IRepository Repository { get; set; }
        public IDynamicExpressionQuery _dynamicExpressionQuery { get; set; }
        public ISessionContext SessionContext { get; set; }
        public IMediaService MediaService { get; set; }
        private readonly ISaveEntityService _saveEntityService;

        public ClinicalExperienceAssetController(IRepository repository,
          IDynamicExpressionQuery dynamicExpressionQuery,
            ISessionContext sessionContext,
            IMediaService mediaService
           )
           
        {
            Repository = repository;
            _dynamicExpressionQuery = dynamicExpressionQuery;
            SessionContext = sessionContext;
            MediaService = mediaService;
        
          
        }

        public ActionResult Display(ViewModel input)
        {
            var viewModel = new ViewModel
                                {
                                    EntityId = 10,
                                    
                                };

            var userEntityId = SessionContext.GetUserEntityId();
          //  var items = _dynamicExpressionQuery.PerformQuery<ClinicalExperience>(null, x => x.User.EntityId == userEntityId, false);
            var exp = Repository.Find<ClinicalExperience>(viewModel.EntityId);
            return View(exp);
        }

      
    }
}