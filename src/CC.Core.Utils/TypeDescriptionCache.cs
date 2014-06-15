using System;
using System.Collections.Generic;
using System.Reflection;

namespace CC.Core.Utilities
{
    public interface ITypeDescriptorCache
    {
        IDictionary<string, PropertyInfo> GetPropertiesFor<T>();
        IDictionary<string, PropertyInfo> GetPropertiesFor(Type itemType);
        void ForEachProperty(Type itemType, Action<PropertyInfo> action);
        void ClearAll();
    }

    public class TypeDescriptorCache : ITypeDescriptorCache
    {
        private static readonly Cache<Type, IDictionary<string, PropertyInfo>> _cache;

        static TypeDescriptorCache()
        {
            _cache = new Cache<Type, IDictionary<string, PropertyInfo>>(type =>
                {
                    var dict = new Dictionary<string, PropertyInfo>();

                    foreach (
                        PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (!propertyInfo.CanWrite) continue;

                        dict.Add(propertyInfo.Name, propertyInfo);
                    }

                    return dict;
                });
        }

        #region ITypeDescriptorCache Members

        public IDictionary<string, PropertyInfo> GetPropertiesFor<T>()
        {
            return GetPropertiesFor(typeof (T));
        }

        public IDictionary<string, PropertyInfo> GetPropertiesFor(Type itemType)
        {
            return _cache[itemType];
        }

        public void ForEachProperty(Type itemType, Action<PropertyInfo> action)
        {
            foreach (PropertyInfo x in _cache[itemType].Values)
            {
                action(x);
            }
        }

        public void ClearAll()
        {
            _cache.ClearAll();
        }

        #endregion

        public static PropertyInfo GetPropertyFor(Type modelType, string propertyName)
        {
            IDictionary<string, PropertyInfo> dict = _cache[modelType];
            return dict.ContainsKey(propertyName) ? dict[propertyName] : null;
        }
    }
}