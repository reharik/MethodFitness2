using Alpinely.TownCrier;
using CC.Core.DomainTools;
using CC.Core.Html.CCUI.HtmlConventionRegistries;
using CC.Core.Html.Grid;
using CC.Core.Localization;
using CC.Core.ValidationServices;
using CC.Security.Interfaces;
using CC.Security.Services;
using CC.UI.Helpers;
using CC.UI.Helpers.Configuration;
using CC.UI.Helpers.Tags;
using MF.Core;
using MF.Core.Config;
using MF.Core.Domain.Tools;
using MF.Web.Config;
using MF.Web.Menus;
using MF.Web.Services;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using StructureMap.Configuration.DSL;

namespace MF.Web.Config
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
                         
                         x.WithDefaultConventions();
                     });
           

            For<HtmlConventionRegistry>().Add<CCHtmlConventions>();
            For<IServiceLocator>().Singleton().Use(new StructureMapServiceLocator());
            For<IElementNamingConvention>().Use<CCElementNamingConvention>();
            For(typeof (ITagGenerator<>)).Use(typeof (TagGenerator<>));
            For<TagProfileLibrary>().Singleton();
            For<ISaveEntityServiceWithoutPrincipal>().Use<NullSaveEntityServiceWithoutPrincipal>();
            For<INHSetupConfig>().Use<NullNHSetupConfig>();

            For<ISessionFactoryConfiguration>().Singleton().Use<NullSqlServerSessionSourceConfiguration>();
            For<ISessionFactory>().Use<NullSessionFactory>();

//            For<ISession>().Use<NullSession>().Named("NoFiltersOrInterceptor");


            For<IUnitOfWork>().Use<NullNHibernateUnitOfWork>();
//            For<IUnitOfWork>().Add<NullNHibernateUnitOfWork>().Named("NoFiltersOrInterceptor");
//            For<IUnitOfWork>().Add<NullNHibernateUnitOfWork>().Named("NoFilters");
            //For<IGetCompanyIdService>().Use<DataLoaderGetCompanyIdService>();

            For<IRepository>().Use<Repository>();
//            For<IRepository>().Add(x => new Repository()).Named("NoFiltersOrInterceptor");
//            For<IRepository>().Add(x => new Repository(true)).Named("NoFilters");

            For<ILocalizationDataProvider>().Use<LocalizationDataProvider>();
            For<IAuthenticationContext>().Use<WebAuthenticationContext>();
            For<IMenuConfig>().Use<MainMenu>();

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
