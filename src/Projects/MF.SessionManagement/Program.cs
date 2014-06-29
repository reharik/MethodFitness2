using CC.Core.DomainTools;
using MF.Core.Services;
using StructureMap;

namespace MF.SessionManagement
{
    class Program
    {
        private static void Main(string[] args)
        {
            Initialize();
            var sessionManager = ObjectFactory.Container.GetInstance<ISessionManager>();
            var repository = ObjectFactory.Container.GetInstance<IRepository>();
            sessionManager.CompleteAppointments();
            repository.Commit();
        }

        private static void Initialize()
        {
            // Bootstrapper.Restart();
//            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new SessionRegistry());
            });
            //ObjectFactory.AssertConfigurationIsValid();


            // HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();


        }

    }
}
