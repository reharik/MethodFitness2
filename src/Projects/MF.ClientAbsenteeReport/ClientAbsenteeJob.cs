using System;
using System.Linq;
using MF.Core;
using Quartz;
using StructureMap;

namespace MF.ClientAbsenteeReport
{
    public class ClientAbsenteeJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
                var logger = ObjectFactory.Container.GetInstance<ILogger>();
                var service = ObjectFactory.Container.GetInstance<IGetDroppedClients>();
                var droppedClients = service.GetClients();
                if (!droppedClients.Any())
                {
                    logger.LogInfo("No new dropped clients on: " + DateTime.Now.ToString());
                    return;
                }
                var email = service.CreateEmail(droppedClients);
                service.SendEmail(email, "absentee report");
                service.UpdateClients(droppedClients);
                logger.LogInfo("Job Processed at: " + DateTime.Now.ToString());
                logger.LogDebug("{0}", email);
        }
    }
}