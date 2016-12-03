using System;
using System.Linq;
using MF.Core;
using Quartz;
using StructureMap;

namespace MF.DailyPaymentReport
{
    public class DailyPaymentJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var service = ObjectFactory.Container.GetInstance<IGetDailyPayments>();
            var logger = ObjectFactory.Container.GetInstance<ILogger>();
            var payments = service.GetPayments();
            if (!payments.Any())
            {
                logger.LogInfo("No payments on: " + DateTime.Now.ToString());
                return;
            }
            var email = service.CreateEmail(payments);
            service.SendEmail(email, "Daily Payment Report");
            logger.LogInfo("Job Processed at: "+ DateTime.Now.ToString());
            logger.LogDebug("{0}", email);
        }
    }
}