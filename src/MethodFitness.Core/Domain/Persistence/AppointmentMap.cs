using FluentNHibernate.Mapping;

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
            Map(x => x.Notes);
            References(x => x.Location);
            References(x => x.Trainer);
            HasManyToMany(x => x.Clients).Access.CamelCaseField(Prefix.Underscore).Cascade.None();
            HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore).Cascade.None();
        } 
    }
}