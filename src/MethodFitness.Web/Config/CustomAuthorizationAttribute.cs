using System.Web.Mvc;
using CC.Core.Html;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Config
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new AjaxAwareRedirectResult(UrlContext.GetUrlForAction<LoginController>(x=>x.Login(null)));
            }
        }
    }

   

}