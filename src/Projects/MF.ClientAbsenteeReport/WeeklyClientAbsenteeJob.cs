using System;
using System.Linq;
using MF.Core;
using Quartz;
using StructureMap;

namespace MF.ClientAbsenteeReport
{
    public class WeeklyClientAbsenteeJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var service = ObjectFactory.Container.GetInstance<IGetDroppedClients>();
            var logger = ObjectFactory.Container.GetInstance<ILogger>();
            var droppedClients = service.GetWeeklyClients();

            var email = service.CreateWeeklyEmail(droppedClients);
            service.SendEmail(email);
            logger.LogDebug("Weekly Job Processed at: "+ DateTime.Now.ToString());
        }
    }
}