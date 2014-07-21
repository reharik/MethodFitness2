namespace MF.Core.Domain.Persistence
{
    public class SessionMap : DomainEntityMap<Session>
    {
        public SessionMap()
        {
            Map(x => x.Date);
            Map(x => x.Cost);
            Map(x => x.AppointmentType);
            Map(x => x.SessionUsed);
            Map(x => x.TrainerPaid);
            Map(x => x.TrainerVerified);
            Map(x => x.PurchaseBatchNumber);
            Map(x => x.TrainerCheckNumber);
            Map(x => x.InArrears);
            References(x => x.Client).Cascade.None();
            References(x => x.Appointment).Cascade.None();
            References(x => x.Trainer);

        } 
    }
}