using Alpinely.TownCrier;
using CC.Core.Core.Domain;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Html.CCUI.HtmlConventionRegistries;
using CC.Core.Core.Html.Grid;
using CC.Core.Core.Localization;
using CC.Core.Core.Services;
using CC.Core.DataValidation;
using CC.Core.Security;
using CC.Core.Security.Interfaces;
using CC.Core.Security.Services;
using CC.Core.UI.Helpers;
using CC.Core.UI.Helpers.Configuration;
using CC.Core.UI.Helpers.Tags;
using MF.Core;
using MF.Core.Config;
using MF.Core.CoreViewModelAndDTOs;
using MF.Core.Domain.Tools;
using MF.Core.Rules;
using MF.Core.Services;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Config;
using MF.Web.Grids;
using MF.Web.Menus;
using MF.Web.Services;
using MF.Web.Services.RouteTokens;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using StructureMap.Configuration.DSL;
using Log4NetLogger = MF.Core.Log4NetLogger;

namespace MF.Web
{
    public class MFWebRegistry : Registry
    {
        public MFWebRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.AssemblyContainingType<MergedEmailFactory>();
                x.AssemblyContainingType<IValidationRunner>();
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
//            For<ISession>().HybridHttpOrThreadLocalScoped().Add(context => context.GetInstance<ISessionFactory>().OpenSession()).Named("NoInterceptorNoFilters");

            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Use<MFUnitOfWork>();
//            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Add<NoInterceptorNoFiltersUnitOfWork>().Named("NoInterceptorNoFilters");

            For<IRepository>().Use<Repository>();
            For<IRepository>().Add<NoFilterRepository>().Named("NoFilters");
//            For<IRepository>().Add<NoInterceptorNoFiltersRepository>().Named("NoInterceptorNoFilters");


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
            For<RulesEngineBase>().Add<DeleteLocationRules>().Named("DeleteLocationRules");

            For<ISessionContext>().Use<SessionContext>();
            For<ICCSessionContext>().Use<SessionContext>();
            For<IMFPermissionsService>().Use<MFPermissionsService>();

            For<IEntityListGrid<TrainerSessionDto>>().Use<SessionVerificationListGrid>().Named("SessionVerification");
            For<IEntityListGrid<TrainerSessionDto>>().Add<SessionPaymentListGrid>().Named("SessionPaymentVerification");
        }
    }
}
