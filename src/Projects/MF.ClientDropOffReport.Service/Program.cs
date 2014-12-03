using StructureMap;
using Topshelf;

namespace MF.ClientDropOffReport.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();
            HostFactory.Run(x =>                                 
            {
                x.Service(() => ObjectFactory.GetInstance<IClientDropOffReport>());
                x.RunAsLocalSystem();                            

                x.SetDescription("Client Drop Off Report");
                x.SetDisplayName("ClientDropOffReport");
                x.SetServiceName("ClientDropOffReport");                       
            });                                                  
        }

        private static void Initialize()
        {
            // Bootstrapper.Restart();
            //            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new ClientDropOffRegistry());
            });
            //            ObjectFactory.AssertConfigurationIsValid();
            // HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();


        }
    }
}
