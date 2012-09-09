using System;
using System.Web.Mvc;
using System.Web.Security;
using Castle.Components.Validator;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Html;
using MethodFitness.Core.Services;
using MethodFitness.Web.Services;
using StructureMap;

namespace MethodFitness.Web.Controllers
{
    public class LoginController:Controller
    {
        private readonly ISecurityDataService _securityDataService;
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IContainer _container;
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;

        public LoginController(ISecurityDataService securityDataService,
            IAuthenticationContext authenticationContext,
            IEmailTemplateService emailTemplateService,
            IContainer container,
            IRepository repository,
            ISaveEntityService saveEntityService)
        {
            _securityDataService = securityDataService;
            _authenticationContext = authenticationContext;
            _emailTemplateService = emailTemplateService;
            _container = container;
            _repository = repository;
            _saveEntityService = saveEntityService;
        }

        public ActionResult Login()
        {
            var loginViewModel = new LoginViewModel
                                     {
                                         _saveUrl = UrlContext.GetUrlForAction<LoginController>(x => x.Login(null))
                                     };
            return View(loginViewModel);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel input)
        {
            var notification = new Notification {Message = WebLocalizationKeys.INVALID_USERNAME_OR_PASSWORD.ToString()};

//            try
//            {
            if (input.UserName.IsNotEmpty() && input.Password.IsNotEmpty())
                {
                    var redirectUrl = string.Empty;
                    var user = _securityDataService.AuthenticateForUserId(input.UserName, input.Password);
                    if (user != null)
                    {
                        redirectUrl = _authenticationContext.ThisUserHasBeenAuthenticated(user, input.RememberMe);
                        notification.Success = true;
                        notification.Message = string.Empty;
                        notification.Redirect = true;
                        notification.RedirectUrl = redirectUrl;
                    }
                }
//            }
//            catch (Exception ex)
//            {
//                notification = new Notification { Message = WebLocalizationKeys.ERROR_UNEXPECTED.ToString() };
//                ex.Source = "CATCH RAISED";
//                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
//            }
            return Json(notification);
        }
            
//            
//            if (input.HasCredentials())
//            {
//                var user = _securityDataService.AuthenticateForUserId(input.UserName, input.Password);
//                if(user!=null)
//                {
//                    var redirectUrl = _authenticationContext.ThisUserHasBeenAuthenticated(user, false);
//                    return Redirect(redirectUrl);
//                }
//            }
//            return Json(notification);
  //      }

        public ActionResult Log_in(LoginViewModel input)
        {
            var user = _repository.Find<User>(input.EntityId);
//            if(user.UserLoginInfo.ByPassToken!=input.Guid)
//            {
//                return RedirectToAction("Login");
//            }
            var redirectUrl = _authenticationContext.ThisUserHasBeenAuthenticated(user,false);
            user.UserLoginInfo.ByPassToken = Guid.Empty;
            var crudManager = _saveEntityService.ProcessSave(user);
            crudManager.Finish();

            return Redirect(redirectUrl);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        
    }

    

    public class LoginViewModel : ViewModel
    {
        [ValidateNonEmpty]
        public string UserName { get; set; }
        [ValidateNonEmpty]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ForgotPasswordUrl { get; set; }
        public string _saveUrl { get; set; }
    }

   
}