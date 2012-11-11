using CC.Core.Services;

namespace MethodFitness.Core.Rules
{
    public class FieldHasNoOutstandingEvents:IRule
    {
        private readonly ISystemClock _systemClock;

        public FieldHasNoOutstandingEvents(ISystemClock systemClock)
        {
            _systemClock = systemClock;
        }

        public ValidationReport Execute<ENTITY>(ENTITY field) where ENTITY : class
        {
            var result = new ValidationReport { Success = true };
//            var count = 0;
//            var _field = field as Field;
//            _field.GetEvents().ForEachItem(x => { if (x .StartTime > _systemClock.Now) count++; });
//            if(count>0)
//            {
//                result.Success = false;
//                result.Message = CoreLocalizationKeys.FIELD_HAS_EVENTS_IN_FUTURE.ToFormat(count);
//            }
            return result;
        }
    }
}