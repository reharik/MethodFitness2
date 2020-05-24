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
            throw new Exception("fuck me");
            var logger = ObjectFactory.Container.GetInstance<ILogger>();
            try
            {
                var service = ObjectFactory.Container.GetInstance<IGetDroppedClients>();
                logger.LogError("about to check for clients");
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
            catch (Exception ex)
            {
                logger.LogError("error", ex);
            }
        }
    }
}