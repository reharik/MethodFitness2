using CC.Core.Domain;
using CC.Core.Services;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using NHibernate;
using NHibernate.Type;
using StructureMap;

namespace MethodFitness.Core.Config
{
    public class SaveUpdateInterceptor : EmptyInterceptor
    {
        public override bool OnFlushDirty(object entity,
                                          object id,
                                          object[] currentState,
                                          object[] previousState,
                                          string[] propertyNames,
                                          IType[] types)
        {
            return OnSave(entity, currentState, propertyNames);
        }
        public override bool OnSave(object entity,
                                    object id,
                                    object[] state,
                                    string[] propertyNames,
                                    IType[] types)
        {
            return OnSave(entity, state, propertyNames);
        }

        private static bool OnSave(object item, object[] state, string[] propertyNames)
        {
            var sessionContext = ObjectFactory.GetInstance<ISessionContext>();
            var systemClock = ObjectFactory.Container.GetInstance<ISystemClock>();

            var entity = item as Entity;
            if (entity != null)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if ("ChangedBy".Equals(propertyNames[i]))
                    {
                        state[i] = sessionContext.GetCurrentUser();
                    } 
                    if ("ChangedDate".Equals(propertyNames[i]))
                    {
                        state[i] = systemClock.Now;
                    }
                    if ("CreatedBy".Equals(propertyNames[i]) && entity.CreatedBy == null)
                    {
                        state[i] = sessionContext.GetCurrentUser();
                    }
                    if ("CreatedDate".Equals(propertyNames[i]) && !entity.CreatedDate.HasValue )
                    {
                        state[i] = systemClock.Now;
                    }
                    
                }
                return true;
            }
            return false;

        }
    }
}