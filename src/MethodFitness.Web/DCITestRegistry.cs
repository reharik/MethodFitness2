using Alpinely.TownCrier;
using AuthorizeNet;
using MethodFitness.Core;
using MethodFitness.Core.Config;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Html.FubuUI.HtmlConventionRegistries;
using MethodFitness.Core.Html.Grid;
using MethodFitness.Core.Localization;
using MethodFitness.Core.Services;
using MethodFitness.Web.Menus;
using MethodFitness.Web.Services;
using FubuMVC.UI;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using KnowYourTurf.Core.Domain;
using KnowYourTurf.Web.Config;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using Rhino.Security.Interfaces;
using Rhino.Security.Services;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace MethodFitness.Web.Config
{
    public class MFTestRegistry : Registry
    {
        public MFTestRegistry()
        {
            Scan(x =>
                     {
                         x.TheCallingAssembly();
                         x.AssemblyContainingType(typeof (CoreLocalizationKeys));
                         x.AssemblyContainingType(typeof (MergedEmailFactory));
                         x.AssemblyContainingType(typeof(Gateway));
                         x.WithDefaultConventions();
                     });
            For<IGateway>().Use<Gateway>().Ctor<string>("apiLogin").EqualToAppSetting("Authorize.Net_apiLogin")
                .Ctor<string>("transactionKey").EqualToAppSetting("Authorize.Net_TransactionKey")
                .Ctor<bool>("testMode").EqualToAppSetting("Authorize.Net_testMode");

            For<HtmlConventionRegistry>().Add<MethodFitnessHtmlConventions>();
            For<IServiceLocator>().Singleton().Use(new StructureMapServiceLocator());
            For<IElementNamingConvention>().Use<MethodFitnessElementNamingConvention>();
            For(typeof (ITagGenerator<>)).Use(typeof (TagGenerator<>));
            For<TagProfileLibrary>().Singleton();
            For<ICastleValidationRunner>().Use<DummyCastleValidationRunnerSuccess>();
            For<ISaveEntityServiceWithoutPrincipal>().Use<NullSaveEntityServiceWithoutPrincipal>();
            For<INHSetupConfig>().Use<NullNHSetupConfig>();

            For<ISessionFactoryConfiguration>().Singleton().Use<NullSqlServerSessionSourceConfiguration>();
            For<ISessionFactory>().Use<NullSessionFactory>();

            For<ISession>().Use<NullSession>();
            For<ISession>().Use<NullSession>().Named("NoFiltersOrInterceptor");


            For<IUnitOfWork>().Use<NullNHibernateUnitOfWork>();
            For<IUnitOfWork>().Add<NullNHibernateUnitOfWork>().Named("NoFiltersOrInterceptor");
            For<IUnitOfWork>().Add<NullNHibernateUnitOfWork>().Named("NoFilters");
            //For<IGetCompanyIdService>().Use<DataLoaderGetCompanyIdService>();

            For<IRepository>().Use<Repository>();
            For<IRepository>().Add(x => new Repository()).Named("NoFiltersOrInterceptor");
            For<IRepository>().Add(x => new Repository(true)).Named("NoFilters");

            For<ILocalizationDataProvider>().Use<LocalizationDataProvider>();
            For<IAuthenticationContext>().Use<WebAuthenticationContext>();
            For<IMenuConfig>().Use<HeaderMenu>();
            For<IMenuConfig>().Add<AssetMenu>().Named("AssetMenu");
            For<IMergedEmailFactory>().LifecycleIs(new UniquePerRequestLifecycle()).Use<MergedEmailFactory>();

            For<IAuthorizationService>().HybridHttpOrThreadLocalScoped().Use<AuthorizationService>();
            For<IAuthorizationRepository>().HybridHttpOrThreadLocalScoped().Use<AuthorizationRepository>();
            For<IPermissionsBuilderService>().HybridHttpOrThreadLocalScoped().Use<PermissionsBuilderService>();
            For<IPermissionsService>().HybridHttpOrThreadLocalScoped().Use<PermissionsService>();
            For(typeof (IGridBuilder<>)).Use(typeof (GridBuilder<>));
            For<ILogger>().Use(() => new NullLogger());


            // RegisterGrids();
        }
    }
}