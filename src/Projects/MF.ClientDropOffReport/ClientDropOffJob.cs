using Quartz;
using StructureMap;

namespace MF.ClientDropOffReport
{
    public class ClientDropOffJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var service = ObjectFactory.Container.GetInstance<IGetDroppedClients>();
            var droppedClients = service.GetClients();
            //            if (!droppedClients.Any())
            //            {
            //                Console.WriteLine("No new dropped clients");
            //                return;
            //            }
            var email = service.CreateEmail(droppedClients);
            service.SendEmail(email);
            service.UpdateClients(droppedClients);
        }
    }
}