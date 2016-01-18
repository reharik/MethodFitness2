using StructureMap;
using Topshelf;

namespace MF.SessionManagement
{
    class Program
    {
        private static void Main(string[] args)
        {
            Initialize();
            HostFactory.Run(x =>
            {
                x.Service(ObjectFactory.GetInstance<IScheduleSetup>);
                x.RunAsLocalSystem();

                x.SetDescription("MF.ClientSessionManager");
                x.SetDisplayName("MF.ClientSessionManager");
                x.SetServiceName("MF.ClientSessionManager");
            });                        
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

