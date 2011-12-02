using System.Collections.Generic;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Html;
using MethodFitness.Core.Html.Menu;
using MethodFitness.Core.Services;
using MethodFitness.Web.Controllers;
using KnowYourTurf.Web.Config;
using StructureMap;

namespace MethodFitness.Web.Areas.Portfolio.Controllers
{
    public class OrthogonalController : MFController
    {
        private readonly IMenuConfig _menuConfig;
        private readonly ISessionContext _sessionContext;
        private readonly IRepository _repository;
        private readonly IContainer _container;

        public OrthogonalController(IMenuConfig menuConfig,
                ISessionContext sessionContext,
            IRepository repository,
            IContainer container)
        {
            _menuConfig = menuConfig;
            _sessionContext = sessionContext;
            _repository = repository;
            _container = container;
        }

        public PartialViewResult MethodFitnessHeader()
        {
            User user=null;
            if (User.Identity.IsAuthenticated)
            {
                user = _repository.Find<User>(_sessionContext.GetUserEntityId());
            }
            HeaderViewModel model = new HeaderViewModel
                                        {
                                            User = user,
                                            LoggedIn = User.Identity.IsAuthenticated,
                                            NotificationSuccessFunction = "mf.popupCrud.controller.success"
                                        };
            return PartialView(model);
        }

        //remove true param when permissions are implemented
        public PartialViewResult MainMenu()
        {
            return PartialView(new MenuViewModel
            {
                MenuItems = _menuConfig.Build(true)
            });
        }
    }

    public class MenuViewModel
    {
        public IList<MenuItem> MenuItems { get; set; }
    }

    public class HeaderViewModel : PartialViewResult
    {
        public string SiteName { get { return CoreLocalizationKeys.SITE_NAME.ToString(); } }
        public User User { get; set; }
        public bool LoggedIn { get; set; }
        public bool IsAdmin { get; set; }
        public bool InAdminMode { get; set; }
        public string UserProfileUrl { get; set; }
        public string NotificationSettingsUrl { get; set; }
        public string NotificationSuccessFunction { get; set; }
    }
}