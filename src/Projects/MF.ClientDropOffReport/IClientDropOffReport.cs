using Quartz;
using Quartz.Impl;
using Topshelf;

namespace MF.ClientDropOffReport
{
    public interface IClientDropOffReport : ServiceControl
    {
        void Execute();
    }

    public class ClientDropOffReport : IClientDropOffReport
    {
        public ClientDropOffReport()
        {
            Execute();
        }

        public void Execute()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<ClientDropOffJob>()
                .WithIdentity("job1", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1").WithCronSchedule("0 0 2 * * ?")
                .StartNow()
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        public bool Start(HostControl hostControl)
        {
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}