using System;
using System.Linq.Expressions;
using CC.Core.Core.Enumerations;
using CC.Core.Core.Html;
using CC.Core.Core.Html.Menu;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Controllers;

namespace MF.Web.Services.RouteTokens
{
    public class RouteToken
    {
        public string url { get; set; }
        public string viewName { get; set; }
        public string subViewName { get; set; }
        public string id { get; set; }
        public string route { get; set; }
        public string addUpdate { get; set; }
        public string display { get; set; }
        public bool isChild { get; set; }
        public bool noBubbleUp { get; set; }
        public bool NoMultiSelectGridView { get; set; }
        [ScriptIgnore]
        public string Operation { get; set; }

        public void CreateUrl<CONTROLLER>(Expression<Func<CONTROLLER, object>> expression, AreaName areaName = null) where CONTROLLER : MFController
        {
            url = UrlContext.GetUrlForAction(expression, areaName);
        }

    }


}