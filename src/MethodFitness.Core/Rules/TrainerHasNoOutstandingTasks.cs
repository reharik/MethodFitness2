using System.Linq;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Controllers;

namespace MethodFitness.Core.Rules
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

        public RuleResult Execute<ENTITY>(ENTITY trainer) where ENTITY : DomainEntity
        {
            var result = new RuleResult {Success = true};
            var _trainer = trainer as User;
            var appointments = _repository.Query<Appointment>(x => x.Trainer == _trainer);
            if (appointments.Any())
            {
                result.Success = false;
                result.Message = CoreLocalizationKeys.TRAINER_HAS_APPOINTMENTS_IN_FUTURE.ToFormat(appointments.Count());
            }
            return result;
        }
    }
}