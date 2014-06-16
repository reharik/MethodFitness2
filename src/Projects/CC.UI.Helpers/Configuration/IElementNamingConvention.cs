using System;
using CC.Core.Utilities;

namespace CC.UI.Helpers.Configuration
{
    public interface IElementNamingConvention
    {
        string GetName(Type modelType, Accessor accessor);
    }
}