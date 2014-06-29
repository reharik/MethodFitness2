using System;
using NHibernate.Proxy;

namespace CC.Core
{
    public static class CoreBasicExtensions
    {
        public static Type GetTypeWhenProxy(this object possibleProxy)
        {
            if (possibleProxy is INHibernateProxy)
            {
                var lazyInitialiser = ((INHibernateProxy) possibleProxy).HibernateLazyInitializer;
                return lazyInitialiser.PersistentClass;
            }
            else
            {
                return possibleProxy.GetType();
            }
        }
    }
}