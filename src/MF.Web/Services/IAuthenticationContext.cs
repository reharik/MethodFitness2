using System;
using System.Web;
using System.Web.Security;
using MF.Core.Domain;

namespace MF.Web.Services
{
    public interface IAuthenticationContext
    {
        string ThisUserHasBeenAuthenticated(User username, bool rememberMe);
        void SignOut();
    }

    public class WebAuthenticationContext : IAuthenticationContext
    {
        public string ThisUserHasBeenAuthenticated(User user,  bool rememberMe)
        {
            string userData = String.Empty;
            userData = userData + "UserId=" + user.EntityId + "|CompanyId=" + user.CompanyId;
            var ticket = new FormsAuthenticationTicket(1, user.FullNameLNF, DateTime.Now, DateTime.Now.AddMonths(10),
                                                       rememberMe, userData);
            string encTicket = FormsAuthentication.Encrypt(ticket);
            var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            if (rememberMe)
                faCookie.Expires = DateTime.Now.AddYears(1); // good for one year
            HttpContext.Current.Response.Cookies.Add(faCookie);
            
            return FormsAuthentication.GetRedirectUrl(user.FullNameLNF, false);
        }

        public void SignOut()
        {
            SignOutFunc();
        }

        public Action<string, bool> SetAuthCookieFunc = FormsAuthentication.SetAuthCookie;
        public Action SignOutFunc = FormsAuthentication.SignOut;
    }
}