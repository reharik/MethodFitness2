using System;
using CC.Core.Core.Html.Grid;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace CC.Core.Core.DomainTools
{
    public class StructuremapGridConvention : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            Type entityType = GetGenericParamFor(type, typeof(Grid<>));

            if (entityType != null)
            {
                var genType = typeof(IGrid<>).MakeGenericType(entityType);
                registry.For(genType).Use(new ConfiguredInstance(type));
            }
        }

        private static Type GetGenericParamFor(Type typeToInspect, Type genericType)
        {
            var baseType = typeToInspect.BaseType;
            if (baseType != null
                && baseType.IsGenericType
                && baseType.GetGenericTypeDefinition().Equals(genericType))
            {
                return baseType.GetGenericArguments()[0];
            }

            return null;
        }
    }
}