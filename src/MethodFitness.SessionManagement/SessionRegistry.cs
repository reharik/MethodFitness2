using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Localization;
using CC.Security;
using MethodFitness.Core;
using MethodFitness.Core.Config;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools;
using MethodFitness.Core.Services;
using NHibernate;
using StructureMap.Configuration.DSL;
namespace MethodFitness.SessionManagement
{
    public class SessionRegistry : Registry
    {
        public SessionRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.AssemblyContainingType(typeof(CoreLocalizationKeys));
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
            For<ILogger>().Use(() => new Core.Log4NetLogger(typeof(string)));

        }
    }
}
