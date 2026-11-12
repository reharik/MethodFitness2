using System;
using System.Linq.Expressions;

namespace CC.Core.Reflection
{
    public interface IValueGetter
    {
        string Name { get; }
        Type DeclaringType { get; }
        object GetValue(object target);

        Expression ChainExpression(Expression body);
    }
}