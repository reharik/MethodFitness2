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

            var domainEntity = item as DomainEntity;
            if (domainEntity != null)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if ("ChangeDate".Equals(propertyNames[i]))
                    {
                        state[i] = systemClock.Now;
                    }
                    if (!domainEntity.CreatedDate.HasValue && "CreateDate".Equals(propertyNames[i]))
                    {
                        state[i] = systemClock.Now;
                    }
                    if ("ChangedBy".Equals(propertyNames[i]))
                    {
                        state[i] = sessionContext.GetCurrentUser();
                    }
                    if ("CompanyId".Equals(propertyNames[i]))
                    {
                        state[i] = sessionContext.GetCompanyId();
                    }
                }
                return true;
            }
           
            var entity = item as Entity;
            if (entity != null)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if ("ChangeDate".Equals(propertyNames[i]))
                    {
                        state[i] = systemClock.Now;
                    }
                    if (!entity.CreatedDate.HasValue && "CreateDate".Equals(propertyNames[i]))
                    {
                        state[i] = systemClock.Now;
                    }
                    if ("ChangedBy".Equals(propertyNames[i]))
                    {
                        state[i] = sessionContext.GetCurrentUser();
                    }
                }
                return true;
            }
            return false;

        }
    }
}