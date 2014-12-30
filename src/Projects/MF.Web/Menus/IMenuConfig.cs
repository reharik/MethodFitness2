using System.Collections.Generic;
using CC.Core.Core.Html.Menu;

namespace MF.Web.Menus
{
    public interface IMenuConfig
    {
        IList<MenuItem> Build(bool withoutPermissions = false);
    }
}