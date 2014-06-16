using System;
using CC.Core.Reflection;

namespace CC.UI.Helpers.Configuration
{
    public interface IElementNamingConvention
    {
        string GetName(Type modelType, Accessor accessor);
    }
}