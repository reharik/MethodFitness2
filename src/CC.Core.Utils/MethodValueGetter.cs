using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CC.Core.Utilities
{
    public class MethodValueGetter : IValueGetter
    {
        private readonly object[] _arguments;
        private readonly MethodInfo _methodInfo;

        public MethodValueGetter(MethodInfo methodInfo, object[] arguments)
        {
            if (arguments.Length > 1)
            {
                throw new NotSupportedException(
                    "ReflectionHelper only supports methods with no arguments or a single indexer argument");
            }

            _methodInfo = methodInfo;
            _arguments = arguments;
        }

        public Type ReturnType
        {
            get { return _methodInfo.ReturnType; }
        }

        #region IValueGetter Members

        public object GetValue(object target)
        {
            return _methodInfo.Invoke(target, _arguments);
        }

        public string Name
        {
            get
            {
                if (_arguments.Length == 1) return string.Format("[{0}]", _arguments[0]);
                if (_arguments.Length == 0) return _methodInfo.Name;

                throw new NotSupportedException("Dunno how to deal with this method");
            }
        }

        public Type DeclaringType
        {
            get { return _methodInfo.DeclaringType; }
        }

        public Expression ChainExpression(Expression body)
        {
            throw new NotSupportedException();
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MethodValueGetter)) return false;
            return Equals((MethodValueGetter) obj);
        }

        public bool Equals(MethodValueGetter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._methodInfo, _methodInfo) && other._arguments.SequenceEqual(_arguments);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (_arguments.Length == 0)
                {
                    return ((_methodInfo != null ? _methodInfo.GetHashCode() : 0)*397) ^
                           (_arguments[0] != null ? _arguments[0].GetHashCode() : 0);
                }

                return _methodInfo.GetHashCode();
            }
        }
    }
}