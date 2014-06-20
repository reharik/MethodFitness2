using Alpinely.TownCrier;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Localization;
using CC.Security;
using CC.Security.Interfaces;
using CC.Security.Services;
using CC.UI.Helpers;
using MF.Core;
using MF.Core.Config;
using MF.Core.Domain.Tools;
using MF.Core.Services;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Menus;
using MF.Web.Services;
using MF.Web.Services.RouteTokens;
using NHibernate;
using StructureMap.Configuration.DSL;
using Log4NetLogger = MF.Core.Log4NetLogger;

namespace Generator
{
    public class GenRegistry : Registry
    {
        public GenRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.ConnectImplementationsToTypesClosing(typeof(IEntityListGrid<>));
                x.AssemblyContainingType(typeof(CoreLocalizationKeys));
                x.AssemblyContainingType(typeof(MergedEmailFactory));
                x.AssemblyContainingType<Entity>();
                x.AssemblyContainingType<IUser>();
                x.AssemblyContainingType<HtmlConventionRegistry>(); 
                x.WithDefaultConventions();
            });

            For<INHSetupConfig>().Use<MFNHSetupConfig>();

            For<ISessionFactoryConfiguration>().Singleton()
                                               .Use<SqlServerSessionSourceConfiguration>()
                                               .Ctor<SqlServerSessionSourceConfiguration>("connectionStr");
            For<ISessionFactory>().Singleton().Use(ctx => ctx.GetInstance<ISessionFactoryConfiguration>().CreateSessionFactory());

            For<ISession>().HybridHttpOrThreadLocalScoped().Use(context => context.GetInstance<ISessionFactory>().OpenSession(new SystemSaveUpdateInterceptor()));

            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Use<UnitOfWork>();

            For<IRepository>().Use<Repository>();

            // no idea why this pos needs to be declared explicitly. very annoying
            // looks like it was because of the capital A.  I renamed it then commented it out.
            // if no problem then I can delete this whole mess.
            //            For<IQADataLoader>().Use<QaDataLoader>();
//            For<IQADataLoader>().Use<QADataLoader>();

            For<ISessionContext>().Use<SystemSessionContext>();

            For<ILocalizationDataProvider>().Use<LocalizationDataProvider>();
            For<IAuthenticationContext>().Use<WebAuthenticationContext>();

            For<IMenuConfig>().Use<MainMenu>();
            For<IRouteTokenConfig>().Add<ScheduleRouteTokenList>();

            For<IAuthorizationService>().HybridHttpOrThreadLocalScoped().Use<AuthorizationService>();
            For<IAuthorizationRepository>().HybridHttpOrThreadLocalScoped().Use<CustomAuthorizationRepository>();
            For<IPermissionsBuilderService>().HybridHttpOrThreadLocalScoped().Use<PermissionsBuilderService>();
            For<IPermissionsService>().HybridHttpOrThreadLocalScoped().Use<PermissionsService>();
            For<ISecuritySetupService>().Use<DefaultSecuritySetupService>();
            For<ILogger>().Use(() => new Log4NetLogger(typeof(string)));

        }
    }
}
