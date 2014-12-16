using Quartz;
using StructureMap;
using Topshelf;

namespace MF.ClientAbsenteeReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();
//            var job = new WeeklyClientAbsenteeJob();
//            job.Execute(null);
//            return;
            HostFactory.Run(x =>                                 
            {
                x.Service(ObjectFactory.GetInstance<IClientAbsenteeReport>);
                x.RunAsLocalSystem();                            

                x.SetDescription("Client Drop Off Report");
                x.SetDisplayName("MF.ClientAbsenteeReport");
                x.SetServiceName("MF.ClientAbsenteeReport");                       
            });                                                  
        }

        private static void Initialize()
        {
            // Bootstrapper.Restart();
            //            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            ObjectFactory.Initialize(x => x.AddRegistry(new ClientAbsenteeRegistry()));
            //            ObjectFactory.AssertConfigurationIsValid();
            // HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();


        }
    }
}
