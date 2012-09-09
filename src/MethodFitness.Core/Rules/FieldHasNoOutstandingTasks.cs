using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;

namespace MethodFitness.Core.Rules
{
    public class FieldHasNoOutstandingTasks:IRule
    {
        public FieldHasNoOutstandingTasks()
        {
        }

        public ValidationReport<ENTITY> Execute<ENTITY>(ENTITY field) where ENTITY : class
        {
            var result = new ValidationReport<ENTITY> { Success = true };
//            var _field = field as Field;
//            var pendingTasks = _field.GetPendingTasks();
//            if(pendingTasks.Count()>0)
//            {
//                result.Success = false;
//                result.Message = CoreLocalizationKeys.FIELD_HAS_TASKS_IN_FUTURE.ToFormat(pendingTasks.Count());
//            }
            return result;
        }
    }
}