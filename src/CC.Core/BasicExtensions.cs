using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using CC.Core.Domain;
using CC.Core.Html;
using CC.Core.Utilities;
using NHibernate.Proxy;

namespace CC.Core
{
    public static class BasicExtensions
    {

        public static bool IsEmpty(this string stringValue)
        {
            return string.IsNullOrEmpty(stringValue);
        }

        public static bool IsNotEmpty(this string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue);
        }

        public static bool ToBool(this string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue)) return false;

            return bool.Parse(stringValue);
        }

        public static string ToFormat(this string stringFormat, params object[] args)
        {
            return String.Format(stringFormat, args);
        }

        public static string If(this string html, Expression<Func<bool>> modelBooleanValue)
        {
            return GetBooleanPropertyValue(modelBooleanValue) ? html : string.Empty;
        }

        public static string IfNot(this string html, Expression<Func<bool>> modelBooleanValue)
        {
            return !GetBooleanPropertyValue(modelBooleanValue) ? html : string.Empty;
        }

        private static bool GetBooleanPropertyValue(Expression<Func<bool>> modelBooleanValue)
        {
            var prop = modelBooleanValue.Body as MemberExpression;
            if (prop != null)
            {
                var info = prop.Member as PropertyInfo;
                if (info != null)
                {
                    return modelBooleanValue.Compile().Invoke();
                }
            }
            throw new ArgumentException("The modelBooleanValue parameter should be a single property, validation logic is not allowed, only 'x => x.BooleanValue' usage is allowed, if more is needed do that in the Controller");
        }

        public static VALUE Get<KEY, VALUE>(this IDictionary<KEY, VALUE> dictionary, KEY key)
        {
            return dictionary.Get(key, default(VALUE));
        }

        public static VALUE Get<KEY, VALUE>(this IDictionary<KEY, VALUE> dictionary, KEY key, VALUE defaultValue)
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return defaultValue;
        }

        public static bool Exists<T>(this IEnumerable<T> values, Func<T, bool> evaluator)
        {
            return values.Count(evaluator) > 0;
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> ForEachItem<T>(this IEnumerable<T> values, Action<T> eachAction)
        {
            foreach (var item in values)
            {
                eachAction(item);
            }

            return values;
        }

        [DebuggerStepThrough]
        public static IEnumerable ForEachItem(this IEnumerable values, Action<object> eachAction)
        {
            foreach (var item in values)
            {
                eachAction(item);
            }

            return values;
        }

        [DebuggerStepThrough]
        public static int IterateFromZero(this int maxCount, Action<int> eachAction)
        {
            for (var idx = 0; idx < maxCount; idx++)
            {
                eachAction(idx);
            }

            return maxCount;
        }

        public static bool HasCustomAttribute<ATTRIBUTE>(this MemberInfo member)
            where ATTRIBUTE : Attribute
        {
            return member.GetCustomAttributes(typeof(ATTRIBUTE), false).Any();
        }

        public static bool IsNullable(this Type theType)
        {
            return (!theType.IsValueType) || theType.IsNullableOfT();
        }

        public static bool IsNullableOfT(this Type theType)
        {
            return theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static bool IsNullableOf(this Type theType, Type otherType)
        {
            return theType.IsNullableOfT() && theType.GetGenericArguments()[0].Equals(otherType);
        }

        public static IList<T> AddMany<T>(this IList<T> list, params T[] items)
        {
            return list.AddRange(items);
        }

        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            items.ForEachItem(list.Add);
            return list;
        }

        public static U ValueOrDefault<T, U>(this T root, Expression<Func<T, U>> expression)
            where T : class
        {
            if (root == null)
            {
                return default(U);
            }

            var accessor = ReflectionHelper.GetAccessor(expression);

            object result = accessor.GetValue(root);

            return (U)(result ?? default(U));
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, propertyName);
        }

        public static IQueryable OrderBy(this IQueryable source, string propertyName)
        {
            return sort(source, propertyName, "OrderBy");
        }

        private static IQueryable sort(IQueryable source, string propertyName, string direction)
        {
            string[] props = propertyName.Split('.');
            Type type = source.ElementType;
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof (Func<,>).MakeGenericType(source.ElementType, type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof (Queryable).GetMethods().Single(
                method => method.Name == direction
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
                .MakeGenericMethod(source.ElementType, type)
                .Invoke(null, new object[] {source, lambda});
            return (IQueryable) result;
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return (IQueryable<T>)OrderByDescending((IQueryable)source, propertyName);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string propertyName)
        {
            return sort(source, propertyName, "OrderByDescending");
        }

        public static void SetValue(this PropertyInfo propertyInfo, object destination, object value)
        {
            if (value == null)
            {
                return;
            }

            Type destType = propertyInfo.PropertyType;

            if (destType == typeof(bool) && Equals(value, propertyInfo.Name))
            {
                value = true;
            }

            if (propertyInfo.PropertyType.IsAssignableFrom(value.GetType()))
            {
                propertyInfo.SetValue(destination, value, null);
                return;
            }

            if (propertyInfo.PropertyType.IsNullableOfT())
            {
                destType = propertyInfo.PropertyType.GetGenericArguments()[0];
            }

            propertyInfo.SetValue(destination, Convert.ChangeType(value, destType), null);
        }



        public static void CallOn<T>(this object target, Action<T> action) where T : class
        {
            var subject = target as T;
            if (subject != null)
            {
                action(subject);
            }
        }

        public static void CallOnEach<T>(this IEnumerable enumerable, Action<T> action) where T : class
        {
            foreach (object o in enumerable)
            {
                o.CallOn(action);
            }
        }

        public static bool IsEntity(this Type type)
        {
            return typeof(Entity).IsAssignableFrom(type);
        }

        public static bool IsDecimal(this string stringValue)
        {
            decimal dec;
            return decimal.TryParse(stringValue, out dec);
        }

        public class InvalidPropertyConversionException : Exception
        {
            public InvalidPropertyConversionException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }

        public static IEnumerable<MethodInfo> NotDefinedOnSystemObject(this MethodInfo[] target)
        {
            foreach (var info in target)
            {
                if (info.DeclaringType.FullName.ToLowerInvariant() != "system.object" && !info.IsSpecialName && !info.IsStatic)
                {
                    yield return info;
                }
            }
        }

        public static string ToSeperateWordsFromPascalCase(this string input)
        {
            return Regex.Replace(input, ".[A-Z]", m => m.ToString()[0] + " " + m.ToString()[1]);
        }

        public static string RemoveWhiteSpace(this string input)
        {
            var item = input.Trim();
            var removeWhiteSpace = Regex.Replace(item, "[ \t]+", "");
            return removeWhiteSpace;
        }

        public static string ToFullUrl(this string relativeUrl, params object[] args)
        {
            var formattedUrl = (args == null) ? relativeUrl : relativeUrl.ToFormat(args);

            return UrlContext.GetFullUrl(formattedUrl);
        }
        
        public static Type GetTypeWhenProxy(this object possibleProxy)
        {
            if (possibleProxy is INHibernateProxy)
            {
                var lazyInitialiser = ((INHibernateProxy)possibleProxy).HibernateLazyInitializer;
                return lazyInitialiser.PersistentClass;
            }
            else
            {
                return possibleProxy.GetType();
            }
        }

        public static DateTime SetTime(this DateTime date, string time)
        {
            return DateTime.Parse(date.ToShortDateString() + " " + time);
        }

        public static DateTime makeDateTime(this string time,DateTime date)
        {
            return DateTime.Parse(date.ToShortDateString() + " " + time);
        }
        public static DateTime makeDateTime(this string time, DateTime? date)
        {
            return DateTime.Parse(date.Value.ToShortDateString() + " " + time);
        }

        public static string AddImageSizeToName(this string imageNameOrUrl, string imageSize)
        {
            return imageNameOrUrl.Insert(imageNameOrUrl.LastIndexOf("."), "_" + imageSize);
        }
            //public static Continuation ToContinuation(this Result result, Continuation continuation)
        //{
        //    List<ErrorInfo> errors = new List<ErrorInfo>();
        //    if(result.Errors!=null)
        //    {
        //        result.Errors.ForEachItem(x => errors.Add(new ErrorInfo(x.Category.ToString(), x.Description)));
        //    }
        //    continuation.Success = result.Success;
        //    continuation.ErrorCount = result.ErrorCount;
        //    continuation.Errors = errors;
        //    return continuation;
        //}
    }
}