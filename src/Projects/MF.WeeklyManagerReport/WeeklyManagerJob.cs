using System;
using System.Linq;
using MF.Core;
using Quartz;
using StructureMap;

namespace MF.WeeklyManagerReport
{
    public class WeeklyManagerJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var service = ObjectFactory.Container.GetInstance<IGetWeeklyManagerReport>();
            var logger = ObjectFactory.Container.GetInstance<ILogger>();
            var payments = service.GetPayments();
            if (!payments.Any())
            {
                logger.LogInfo("No payments on: " + DateTime.Now.ToString());
                return;
            }
            var email = service.CreateEmail(payments);
            service.SendEmail(email, "Weekly Manager Report");
            logger.LogInfo("Job Processed at: "+ DateTime.Now.ToString());
            logger.LogDebug("{0}", email);
        }
    }
}