using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;

namespace MethodFitness.Core.Rules
{
    public class FieldHasNoOutstandingEvents:IRule
    {
        private readonly ISystemClock _systemClock;

        public FieldHasNoOutstandingEvents(ISystemClock systemClock)
        {
            _systemClock = systemClock;
        }

        public ValidationReport<ENTITY> Execute<ENTITY>(ENTITY field) where ENTITY : DomainEntity
        {
            var result = new ValidationReport<ENTITY> { Success = true };
//            var count = 0;
//            var _field = field as Field;
//            _field.GetEvents().Each(x => { if (x .StartTime > _systemClock.Now) count++; });
//            if(count>0)
//            {
//                result.Success = false;
//                result.Message = CoreLocalizationKeys.FIELD_HAS_EVENTS_IN_FUTURE.ToFormat(count);
//            }
            return result;
        }
    }
}