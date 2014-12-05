using System;
using System.Linq;
using MF.Core;
using Quartz;
using StructureMap;

namespace MF.ClientDropOffReport
{
    public class ClientDropOffJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var service = ObjectFactory.Container.GetInstance<IGetDroppedClients>();
            var logger = ObjectFactory.Container.GetInstance<ILogger>();
            var droppedClients = service.GetClients();
            if (!droppedClients.Any())
            {
                logger.LogDebug("No new dropped clients on: "+ DateTime.Now.ToString());
                return;
            }
            var email = service.CreateEmail(droppedClients);
            service.SendEmail(email);
            service.UpdateClients(droppedClients);
            logger.LogDebug("Job Processed at: "+ DateTime.Now.ToString());
        }
    }
}