using FluentNHibernate.Mapping;
using MethodFitness.Web.Areas.Schedule.Controllers;

namespace MethodFitness.Core.Domain.Persistence
{
    public class AppointmentMap : DomainEntityMap<Appointment>
    {
        public AppointmentMap()
        {
            Map(x => x.ScheduledDate);
            Map(x => x.ScheduledStartTime);
            Map(x => x.ScheduledEndTime);
            Map(x => x.Client);
            References(x => x.Location);
            References(x => x.Trainer);
        } 
    }
}