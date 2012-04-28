using System.Collections.Generic;
using MethodFitness.Core.Html.Menu;

namespace MethodFitness.Web.Config
{
    public interface IMenuConfig
    {
        IList<MenuItem> Build(bool withoutPermissions = false);
    }
}