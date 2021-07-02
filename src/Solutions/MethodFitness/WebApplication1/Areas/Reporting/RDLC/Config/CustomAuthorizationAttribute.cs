using CC.Core.Core.Html;
using CC.Core.Core.Html.Menu;
using MF.Core.Services;
using MF.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MF.Web.Config
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is UnauthorizedResult)
            {
                filterContext.Result = new AjaxAwareRedirectResult(UrlContext.GetUrlForAction<LoginController>(x=>x.Login(null)));
            }
        }
    }

   

}