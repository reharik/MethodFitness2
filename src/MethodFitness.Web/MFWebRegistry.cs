using Alpinely.TownCrier;
using AuthorizeNet;
using MethodFitness.Core;
using MethodFitness.Core.Config;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Html.FubuUI.HtmlConventionRegistries;
using MethodFitness.Core.Html.Grid;
using MethodFitness.Core.Localization;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Menus;
using MethodFitness.Web.Services;
using FubuMVC.UI;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using KnowYourTurf.Web.Config;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using Rhino.Security.Interfaces;
using Rhino.Security.Services;
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
                x.ConnectImplementationsToTypesClosing(typeof(IEntityListGrid<>));
                x.AssemblyContainingType(typeof(CoreLocalizationKeys));
                x.AssemblyContainingType(typeof(MergedEmailFactory));
                x.AssemblyContainingType(typeof(Gateway));
                x.WithDefaultConventions();
            });
            For<IGateway>().Use<Gateway>().Ctor<string>("apiLogin").EqualToAppSetting("Authorize.Net_apiLogin")
                .Ctor<string>("transactionKey").EqualToAppSetting("Authorize.Net_TransactionKey")
                .Ctor<bool>("testMode").EqualToAppSetting("Authorize.Net_testMode");

            For<HtmlConventionRegistry>().Add<MethodFitnessHtmlConventions>();
            For<IServiceLocator>().Singleton().Use(new StructureMapServiceLocator());
            For<IElementNamingConvention>().Use<MethodFitnessElementNamingConvention>();
            For(typeof(ITagGenerator<>)).Use(typeof(TagGenerator<>));
            For<TagProfileLibrary>().Singleton();
            For<INHSetupConfig>().Use<MFNHSetupConfig>();

            For<ISessionFactoryConfiguration>().Singleton()
                .Use<SqlServerSessionSourceConfiguration>()
                .Ctor<SqlServerSessionSourceConfiguration>("connectionStr")
                .EqualToAppSetting("MethodFitness.sql_server_connection_string");
            For<ISessionFactory>().Singleton().Use(ctx => ctx.GetInstance<ISessionFactoryConfiguration>().CreateSessionFactory());

            For<ISession>().HybridHttpOrThreadLocalScoped().Use(context => context.GetInstance<ISessionFactory>().OpenSession(new SaveUpdateInterceptor()));
            For<ISession>().HybridHttpOrThreadLocalScoped().Add(context => context.GetInstance<ISessionFactory>().OpenSession()).Named("NoFiltersOrInterceptor");

            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Use<UnitOfWork>();
            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Add(context => new UnitOfWork()).Named("NoFilters");
            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Add(context => new UnitOfWork(true)).Named("NoFiltersOrInterceptor");

            For<IRepository>().Use<Repository>();
            For<IRepository>().Add(x => new Repository(true)).Named("NoFiltersOrInterceptor");
            For<IRepository>().Add(x => new Repository()).Named("NoFilters");


            For<IMergedEmailFactory>().Use<MergedEmailFactory>();
            For<ITemplateParser>().Use<TemplateParser>();

            For<ILocalizationDataProvider>().Use<LocalizationDataProvider>();
            For<IAuthenticationContext>().Use<WebAuthenticationContext>();

            For<IMenuConfig>().Use<MainMenu>();
            For<IMergedEmailFactory>().LifecycleIs(new UniquePerRequestLifecycle()).Use<MergedEmailFactory>();

            For<IAuthorizationService>().HybridHttpOrThreadLocalScoped().Use<AuthorizationService>();
            For<IAuthorizationRepository>().HybridHttpOrThreadLocalScoped().Use<AuthorizationRepository>();
            For<IPermissionsBuilderService>().HybridHttpOrThreadLocalScoped().Use<PermissionsBuilderService>();
            For<IPermissionsService>().HybridHttpOrThreadLocalScoped().Use<PermissionsService>();

            For(typeof(IGridBuilder<>)).Use(typeof(GridBuilder<>));
            
            For<ILogger>().Use(() => new Log4NetLogger(typeof(string)));

        }
    }
}
