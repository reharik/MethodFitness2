using FluentNHibernate.Mapping;

namespace MethodFitness.Core.Domain.Persistence
{
    public class SessionMap : DomainEntityMap<Session>
    {
        public SessionMap()
        {
            Map(x => x.Date);
            Map(x => x.Cost);
            Map(x => x.AppointmentType);
            Map(x => x.SessionCompleted);
            Map(x => x.TrainerPaid);
            Map(x => x.PurchaseBatchNumber);
            Map(x => x.TrainerCheckNumber);
            Map(x => x.InArrears);
            References(x => x.Client);
            References(x => x.Appointment);
            References(x => x.Trainer);

        } 
    }
}