using System;

namespace CC.Core.Utilities
{
    public static class TypeExtensions
    {
        public static bool CanBeCastTo<T>(this Type type)
        {
            if (type == null)
                return false;
            if (type == typeof (T))
                return true;
            else
                return typeof (T).IsAssignableFrom(type);
        }

        public static bool IsInNamespace(this Type type, string nameSpace)
        {
            return type.Namespace.StartsWith(nameSpace);
        }

        public static bool IsInNamespaceContaining<T>(this Type type)
        {
            return type.IsInNamespace(typeof (T).Namespace);
        }

        public static bool IsOpenGeneric(this Type type)
        {
            return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        }

        public static bool IsConcreteTypeOf(this Type pluggedType, Type pluginType)
        {
            return pluggedType.IsConcrete() && pluginType.IsAssignableFrom(pluggedType);
        }

        public static bool IsConcreteTypeOf<T>(this Type pluggedType)
        {
            return pluggedType.IsConcrete() && typeof (T).IsAssignableFrom(pluggedType);
        }

        public static bool ImplementsInterfaceTemplate(this Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete())
                return false;
            foreach (Type type in pluggedType.GetInterfaces())
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == templateType)
                    return true;
            }
            return false;
        }

        public static Type FindInterfaceThatCloses(this Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete())
                return null;
            foreach (Type type in pluggedType.GetInterfaces())
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == templateType)
                    return type;
            }
            return pluggedType.BaseType == typeof (object)
                       ? null
                       : pluggedType.BaseType.FindInterfaceThatCloses(templateType);
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        public static bool Closes(this Type type, Type openType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == openType)
                return true;
            Type baseType = type.BaseType;
            if (baseType == null)
                return false;
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == openType)
                return true;
            else
                return type.BaseType != null && type.BaseType.Closes(openType);
        }

        public static Type GetInnerTypeFromNullable(this Type nullableType)
        {
            return nullableType.GetGenericArguments()[0];
        }

        public static string GetName(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;
            string str = string.Join(", ", Array.ConvertAll(type.GetGenericArguments(), (t => t.GetName())));
            return string.Format("{0}<{1}>", type.Name, str);
        }

        public static string GetFullName(this Type type)
        {
            if (!type.IsGenericType)
                return type.FullName;
            string str = string.Join(", ", Array.ConvertAll(type.GetGenericArguments(), (t => t.GetName())));
            return string.Format("{0}<{1}>", type.Name, str);
        }

        public static bool IsString(this Type type)
        {
            return type == typeof (string);
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive && !type.IsString() && type != typeof (IntPtr);
        }

        public static bool IsSimple(this Type type)
        {
            return type.IsPrimitive || type.IsString() || type.IsEnum;
        }

        public static bool IsConcrete(this Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        public static bool IsNotConcrete(this Type type)
        {
            return !type.IsConcrete();
        }
    }
}