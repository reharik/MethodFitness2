using Quartz;
using StructureMap;
using Topshelf;

namespace MF.WeeklyManagerReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();
            HostFactory.Run(x =>                                 
            {
                x.Service(ObjectFactory.GetInstance<IWeeklyManagerReport>);
                x.RunAsLocalSystem();                            

                x.SetDescription("Weekly Manager Report");
                x.SetDisplayName("MF.WeeklyManagerReport");
                x.SetServiceName("MF.WeeklyManagerReport");                       
            });                                                  
        }

        private static void Initialize()
        {
            ObjectFactory.Initialize(x => x.AddRegistry(new WeeklyManagerRegistry()));
        }
    }
}
