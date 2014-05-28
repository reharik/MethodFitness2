using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CC.Core.Utilities
{
    public interface Accessor
    {
        string FieldName { get; }

        Type PropertyType { get; }
        PropertyInfo InnerProperty { get; }
        Type DeclaringType { get; }
        string Name { get; }
        Type OwnerType { get; }
        string[] PropertyNames { get; }
        void SetValue(object target, object propertyValue);
        object GetValue(object target);

        Accessor GetChildAccessor<T>(Expression<Func<T, object>> expression);

        Expression<Func<T, object>> ToExpression<T>();

        Accessor Prepend(PropertyInfo property);

        IEnumerable<IValueGetter> Getters();
    }

    public static class AccessorExtensions
    {
        public static Accessor Prepend(this Accessor accessor, Accessor prefixedAccessor)
        {
            return new PropertyChain(prefixedAccessor.Getters().Union(accessor.Getters()).ToArray());
        }
    }
}