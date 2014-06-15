using System.Linq;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Services;
using MethodFitness.Core.Domain;
using xVal.ServerSide;

namespace MethodFitness.Core.Rules
{
    public class NoTrainerHasBeenPaidForPaymentSessions : IRule
    {
        private readonly ISystemClock _systemClock;
        private readonly IRepository _repository;

        public NoTrainerHasBeenPaidForPaymentSessions(ISystemClock systemClock, IRepository repository)
        {
            _systemClock = systemClock;
            _repository = repository;
        }

        public ValidationReport Execute<ENTITY>(ENTITY trainer) where ENTITY : Entity
        {
            var result = new ValidationReport { Success = true };
//            var _trainer = trainer as User;
//            var appointments = _repository.Query<Appointment>(x => x.Trainer == _trainer);
//            if (appointments.Any())
//            {
//                result.Success = false;
//                result.AddErrorInfo(new ErrorInfo("Rule", CoreLocalizationKeys.TRAINER_HAS_APPOINTMENTS_IN_FUTURE.ToFormat(appointments.Count())));
//            }
            return result;
        }
    }
}