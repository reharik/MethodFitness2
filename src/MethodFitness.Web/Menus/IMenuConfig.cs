using System.Collections.Generic;
using CC.Core.Html.Menu;

namespace MethodFitness.Web.Config
{
    public interface IMenuConfig
    {
        IList<MenuItem> Build(bool withoutPermissions = false);
    }
}