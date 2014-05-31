using Alpinely.TownCrier;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Localization;
using CC.Security;
using CC.Security.Interfaces;
using CC.Security.Services;
using CC.UI.Helpers;
using MethodFitness.Core.Config;
using MethodFitness.Core.Services;
using MethodFitness.Web.Config;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Menus;
using MethodFitness.Web.Services;
using MethodFitness.Web.Services.ViewOptions;
using NHibernate;
using StructureMap.Configuration.DSL;
using Log4NetLogger = MethodFitness.Core.Log4NetLogger;

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
