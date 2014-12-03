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
            // define the job and tie it to our HelloJob class
            var job = JobBuilder.Create<ClientDropOffJob>()
                .WithIdentity("job1", "group1")
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow().WithDailyTimeIntervalSchedule(x => x.StartingDailyAt(new TimeOfDay(1, 0)))
                .Build();

            // Tell quartz to schedule the job using our trigger
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