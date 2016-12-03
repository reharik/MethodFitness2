using Quartz;
using StructureMap;
using Topshelf;

namespace MF.DailyPaymentReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();
            HostFactory.Run(x =>                                 
            {
                x.Service(ObjectFactory.GetInstance<IDailyPaymentReport>);
                x.RunAsLocalSystem();                            

                x.SetDescription("Daily Payment Report");
                x.SetDisplayName("MF.DailyPaymentReport");
                x.SetServiceName("MF.DailyPaymentReport");                       
            });                                                  
        }

        private static void Initialize()
        {
            ObjectFactory.Initialize(x => x.AddRegistry(new DailyPaymentRegistry()));
        }
    }
}
