using System;
using System.Linq;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Services;
using CC.Core.ValidationServices;
using CC.DataValidation;
using MF.Core.Domain;

namespace MF.Core.Rules
{
    public class TrainerHasNoOutstandingAppointments : IRule
    {
        private readonly ISystemClock _systemClock;
        private readonly IRepository _repository;

        public TrainerHasNoOutstandingAppointments(ISystemClock systemClock, IRepository repository)
        {
            _systemClock = systemClock;
            _repository = repository;
        }

        public ValidationReport Execute<ENTITY>(ENTITY trainer) where ENTITY : Entity
        {
            var result = new ValidationReport { Success = true };
            var _trainer = trainer as User;
            var appointments = _trainer.Appointments.Where(x=>x.StartTime> DateTime.Now);
            if (appointments.Any())
            {
                result.Success = false;
                result.AddErrorInfo(new ErrorInfo("Rule", CoreLocalizationKeys.TRAINER_HAS_APPOINTMENTS_IN_FUTURE.ToFormat(appointments.Count())));
            }
            return result;
        }
    }
}