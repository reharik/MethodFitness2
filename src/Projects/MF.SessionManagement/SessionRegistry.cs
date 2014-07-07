using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Localization;
using CC.DataValidation;
using CC.Security;
using MF.Core;
using MF.Core.Config;
using MF.Core.Domain.Tools;
using MF.Core.Services;
using NHibernate;
using StructureMap.Configuration.DSL;
using Log4NetLogger = MF.Core.Log4NetLogger;

namespace MF.SessionManagement
{
    public class SessionRegistry : Registry
    {
        public SessionRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.AssemblyContainingType<IValidationRunner>();
                x.AssemblyContainingType<CoreLocalizationKeys>();
                x.AssemblyContainingType<Entity>();
                x.AssemblyContainingType<IUser>();
                x.WithDefaultConventions();
            });

            For<INHSetupConfig>().Use<MFNHSetupConfig>();

            For<ISessionFactoryConfiguration>().Singleton()
                                               .Use<SqlServerSessionSourceConfiguration>()
                                               .Ctor<SqlServerSessionSourceConfiguration>("connectionStr")
                                                .EqualToAppSetting("MethodFitness.sql_server_connection_string");
            For<ISessionFactory>().Singleton().Use(ctx => ctx.GetInstance<ISessionFactoryConfiguration>().CreateSessionFactory());

            For<ISession>().HybridHttpOrThreadLocalScoped().Use(context => context.GetInstance<ISessionFactory>().OpenSession(new SystemSaveUpdateInterceptor()));
            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Use<MFUnitOfWork>();
            For<IRepository>().Use<Repository>();

            For<ISessionContext>().Use<SystemSessionContext>();
            For<ILocalizationDataProvider>().Use<LocalizationDataProvider>();
            For<ILogger>().Use(() => new Log4NetLogger(typeof(string)));

        }
    }
}
