using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CC.DataValidation.Attributes;
using CC.Utility;
using NHibernate;

namespace CC.DataValidation
{
    public interface IValidationRunner
    {
        IEnumerable<ErrorInfo> Validate(object target);
    }

    public class ValidationRunner : IValidationRunner
    {
        public IEnumerable<ErrorInfo> Validate(object target)
        {
            var _target = (target.GetType().IsGenericType && target.GetType().GetInterfaces().Any(x=>x == typeof (IEnumerable)))
                         ? (IEnumerable<object>) target
                         : new[] {target};

            foreach (var instance in _target)
            {
                foreach (var prop in TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>())
                {
                    var value = prop.GetValue(instance);
                    foreach (var errorInfo in runChildOrChildren(value, prop))
                    {
                        yield return errorInfo;
                    }

                    foreach (var errorInfo in validateFirstLevelProperties(prop, value, instance))
                    {
                        yield return errorInfo;
                    }
                }
            }
        }

        private IEnumerable<ErrorInfo> runChildOrChildren(object value, PropertyDescriptor prop)
        {
            var processCollection = false;
            new List<Func<PropertyDescriptor, object, bool>>
                {
                    (p, v) => NHibernateUtil.IsInitialized(v),
                    (p, v) => v != null,
                    (p, v) => !p.Attributes.OfType<DoNotValidateAttribute>().Any(),
                    (p, v) => p.PropertyType.IsGenericType,
                    (p, v) => p.PropertyType.GetGenericTypeDefinition() == typeof (IEnumerable<>) || p.PropertyType.GetGenericTypeDefinition() == typeof (List<>) 
                }.ForEachItemWithBreak(expr => { processCollection = expr.Invoke(prop, value); return !processCollection; });
            var processReference = false;
            new List<Func<PropertyDescriptor, object, bool>>
                {
                    (p, v) => v != null,
                    (p, v) => !p.Attributes.OfType<DoNotValidateAttribute>().Any(),
                    (p, v) => !p.PropertyType.IsGenericType,
                    (p, v) => !p.PropertyType.IsPrimitive,
                    (p, v) => !p.PropertyType.IsValueType,
                    (p, v) => p.PropertyType != typeof (string)
                }.ForEachItemWithBreak(expr => { processReference = expr.Invoke(prop,value); return !processReference; });

            if (!processCollection && !processReference) yield break;
            foreach (var errorInfo in Validate(value))
            {
                yield return errorInfo;
            }
        }

        private IEnumerable<ErrorInfo> validateFirstLevelProperties(PropertyDescriptor prop, object value, object instance)
        {
            return prop.Attributes.OfType<ValidationAttribute>()
                .Where(attr => !attr.IsValid(value))
                .Select(inValid =>
                    new ErrorInfo(prop.Name, inValid.FormatErrorMessage(prop.Name.ToSeperateWordsFromPascalCase()))
                    {
                        Instance = instance,
                        ObjectType = instance.GetType().Name
                    });
        }
    }
}
