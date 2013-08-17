using Alpinely.TownCrier;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Html.CCUI.HtmlConventionRegistries;
using CC.Core.Html.Grid;
using CC.Core.Localization;
using CC.Core.Services;
using CC.Security;
using CC.Security.Interfaces;
using CC.Security.Services;
using CC.UI.Helpers;
using CC.UI.Helpers.Configuration;
using CC.UI.Helpers.Tags;
using MethodFitness.Core;
using MethodFitness.Core.Config;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Rules;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Grids;
using MethodFitness.Web.Menus;
using MethodFitness.Web.Services;
using MethodFitness.Web.Services.ViewOptions;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using Log4NetLogger = MethodFitness.Core.Log4NetLogger;

namespace MethodFitness.Web
{
    public class MFWebRegistry : Registry
    {
        public MFWebRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.AssemblyContainingType<MergedEmailFactory>();
                x.AssemblyContainingType<CoreLocalizationKeys>();
                x.AssemblyContainingType<Entity>();
                x.AssemblyContainingType<IUser>();
                x.AssemblyContainingType<HtmlConventionRegistry>();
                x.ConnectImplementationsToTypesClosing(typeof(IEntityListGrid<>));
                x.WithDefaultConventions();
            });

            For<HtmlConventionRegistry>().Add<CCHtmlConventions2>();
            For<IServiceLocator>().Singleton().Use(new StructureMapServiceLocator());
            For<IElementNamingConvention>().Use<CCElementNamingConvention>();
            For(typeof(ITagGenerator<>)).Use(typeof(TagGenerator<>));
            For<TagProfileLibrary>().Singleton();
            For<INHSetupConfig>().Use<MFNHSetupConfig>();
            For<ISessionFactoryConfiguration>().Singleton()
                .Use<SqlServerSessionSourceConfiguration>()
                .Ctor<SqlServerSessionSourceConfiguration>("connectionStr")
                .EqualToAppSetting("MethodFitness.sql_server_connection_string");
            For<ISessionFactory>().Singleton().Use(ctx => ctx.GetInstance<ISessionFactoryConfiguration>().CreateSessionFactory());
  
            For<ISession>().HybridHttpOrThreadLocalScoped().Use(context => context.GetInstance<ISessionFactory>().OpenSession(new SaveUpdateInterceptor()));
            For<ISession>().HybridHttpOrThreadLocalScoped().Add(context => context.GetInstance<ISessionFactory>().OpenSession()).Named("NoInterceptorNoFilters");

            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Use<MFUnitOfWork>();
            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Add<NoInterceptorNoFiltersUnitOfWork>().Named("NoInterceptorNoFilters");

            For<IRepository>().Use<Repository>();
            For<IRepository>().Add<NoFilterRepository>().Named("NoFilters");
            For<IRepository>().Add<NoInterceptorNoFiltersRepository>().Named("NoInterceptorNoFilters");


            For<ITemplateParser>().Use<TemplateParser>();

            For<ILocalizationDataProvider>().Use<LocalizationDataProvider>();
            For<IAuthenticationContext>().Use<WebAuthenticationContext>();

            For<IMenuConfig>().Use<MainMenu>();

            For<IAuthorizationService>().HybridHttpOrThreadLocalScoped().Use<AuthorizationService>();
            For<IAuthorizationRepository>().HybridHttpOrThreadLocalScoped().Use<CustomAuthorizationRepository>();
            For<IPermissionsBuilderService>().HybridHttpOrThreadLocalScoped().Use<PermissionsBuilderService>();
            For<IPermissionsService>().HybridHttpOrThreadLocalScoped().Use<PermissionsService>();
            For<ISecuritySetupService>().Use<DefaultSecuritySetupService>();
            For<IRouteTokenConfig>().Add<ScheduleRouteTokenList>();

            For(typeof(IGridBuilder<>)).Use(typeof(GridBuilder<>));
            
            For<ILogger>().Use(() => new Log4NetLogger(typeof(string)));
            For<RulesEngineBase>().Use<DeleteEmployeeRules>().Named("DeleteClientRules");
            For<RulesEngineBase>().Add<DeleteTrainerRules>().Named("DeleteTrainerRules");

            For<ISessionContext>().Use<SessionContext>();
            For<ICCSessionContext>().Use<SessionContext>();
            For<IMFPermissionsService>().Use<MFPermissionsService>();

            For<IEntityListGrid<TrainerSessionDto>>().Use<SessionVerificationListGrid>().Named("SessionVerification");
            For<IEntityListGrid<TrainerSessionDto>>().Add<SessionPaymentListGrid>().Named("SessionPaymentVerification");
        }
    }
}
