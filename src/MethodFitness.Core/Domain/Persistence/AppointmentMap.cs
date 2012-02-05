using FluentNHibernate.Mapping;
using MethodFitness.Web.Areas.Schedule.Controllers;

namespace MethodFitness.Core.Domain.Persistence
{
    public class AppointmentMap : DomainEntityMap<Appointment>
    {
        public AppointmentMap()
        {
            Map(x => x.Date);
            Map(x => x.StartTime);
            Map(x => x.EndTime);
            Map(x => x.Length);
            Map(x => x.Completed);
            References(x => x.Location);
            References(x => x.Trainer);
            HasManyToMany(x => x.Clients).Access.CamelCaseField(Prefix.Underscore).Cascade.None();
            HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore).Cascade.None();
        } 
    }
}