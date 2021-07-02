using FluentNHibernate.Mapping;
using MF.Core.CoreViewModelAndDTOs;

namespace MF.Core.Domain.Persistence
{
    public class TrainerSessionDtoMap :  ClassMap<TrainerSessionDto>
    {
        public TrainerSessionDtoMap()
        {
          Table("TrainerSessions");
            Id(x => x.EntityId);
            Map(x => x.TrainerId);
            Map(x => x.TrainerSessionVerificationId);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.AppointmentDate);
            Map(x => x.Type);
            Map(x => x.PricePerSession);
            Map(x => x.TrainerPercentage);
            Map(x => x.TrainerPaid);
            Map(x => x.InArrears);
            Map(x => x.TrainerVerified);
        } 
    }
}