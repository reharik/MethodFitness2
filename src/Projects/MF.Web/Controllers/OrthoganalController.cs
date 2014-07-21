using System.Collections.Generic;
using System.Web.Mvc;
using CC.Core.DomainTools;
using CC.Core.Html.Menu;
using MF.Core;
using MF.Core.Domain;
using MF.Core.Services;
using MF.Web.Menus;
using StructureMap;

namespace MF.Web.Controllers
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
                user = _repository.Find<User>(_sessionContext.GetUserId());
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
        public ActionResult MainMenu()
        {
            return View(new MenuViewModel
            {
                MenuItems = _menuConfig.Build()
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