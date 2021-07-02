using System;
using System.Threading;
using CC.Core.Core.DomainTools;
using MF.Core.Config;
using MF.Web.Config;
using StructureMap;
using MF.Core;
using Microsoft.AspNetCore.Http;
namespace MF.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("elmah.axd");
            routes.MapRoute(
               "Scheduler",
               "Scheduler",
               new { controller = "MethodFitness", action = "Home" }); 
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{EntityId}", // URL with parameters
                new { controller = "Login", action = "Login", EntityId = UrlParameter.Optional } // Parameter defaults
            );
        }

        void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            var logger = ObjectFactory.GetInstance<ILogger>();
            logger.LogError(ex.Message, ex);
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
            Bootstrapper.Bootstrap();
        }

        protected void Application_EndRequest()
        {
            var unitOfWork = ObjectFactory.Container.GetInstance<IUnitOfWork>();
            unitOfWork.Dispose();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = HttpContextHelper.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return;
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var userData = authTicket.UserData;

            CustomIdentitiy identity = new CustomIdentitiy(authTicket);
            CustomPrincipal principal = new CustomPrincipal(identity, userData);
            HttpContextHelper.Current.User = principal;
            Thread.CurrentPrincipal = principal;
        }
    }

    
}