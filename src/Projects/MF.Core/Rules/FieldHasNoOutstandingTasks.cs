using CC.Core.Core.Domain;
using CC.Core.Core.ValidationServices;

namespace MF.Core.Rules
{
    public class FieldHasNoOutstandingTasks:IRule
    {
        public FieldHasNoOutstandingTasks()
        {
        }

        public ValidationReport Execute<ENTITY>(ENTITY field) where ENTITY : Entity
        {
            var result = new ValidationReport { Success = true };
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