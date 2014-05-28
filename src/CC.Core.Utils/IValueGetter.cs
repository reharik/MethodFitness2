using System;
using System.Linq.Expressions;

namespace CC.Core.Utilities
{
    public interface IValueGetter
    {
        string Name { get; }
        Type DeclaringType { get; }
        object GetValue(object target);

        Expression ChainExpression(Expression body);
    }
}