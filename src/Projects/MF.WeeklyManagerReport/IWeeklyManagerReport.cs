using System;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace MF.WeeklyManagerReport
{
    public interface IWeeklyManagerReport : ServiceControl
    {
        void Execute();
    }

    public class WeeklyManagerReport : IWeeklyManagerReport
    {
        public WeeklyManagerReport()
        {
            Execute();
        }

        public void Execute()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<WeeklyManagerJob>()
                .WithIdentity("job1", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithCronSchedule("0 10 2 ? * SUN")
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