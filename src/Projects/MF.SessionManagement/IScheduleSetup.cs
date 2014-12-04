using System;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace MF.SessionManagement
{
    public interface IScheduleSetup : ServiceControl
    {
    }

    public class ScheduleSetup : IScheduleSetup
    {
        public ScheduleSetup()
        {
            Execute();
        }

        public void Execute()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            // define the job and tie it to our HelloJob class
            var job = JobBuilder.Create<ClientSessionJob>()
                .WithIdentity("job1", "group1")
                .Build();
            var rawTime = DateTime.Now;
            var startTime = new DateTime(
                         rawTime.Year,
                         rawTime.Month,
                         rawTime.Day,
                         (rawTime.Minute < 30) ? rawTime.Hour : rawTime.Hour + 1,
                         (rawTime.Minute < 40) ? 40 : 10,
                         0
                     );
            // Trigger the job to run now, and then repeat every 10 seconds
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(new DateTimeOffset(startTime))
                .WithDailyTimeIntervalSchedule(x => 
                    x.WithIntervalInMinutes(30).StartingDailyAt(new TimeOfDay(4,10)).EndingDailyAt(new TimeOfDay(23,11)))
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