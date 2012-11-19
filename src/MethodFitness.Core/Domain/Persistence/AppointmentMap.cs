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
            Map(x => x.AppointmentType);
            Map(x => x.Completed);
            References(x => x.Location).Not.LazyLoad();
            References(x => x.Trainer).Not.LazyLoad();
            HasManyToMany(x => x.Clients).Not.LazyLoad().Access.CamelCaseField(Prefix.Underscore).Cascade.None();
            HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore).Cascade.None();
        } 
    }
}