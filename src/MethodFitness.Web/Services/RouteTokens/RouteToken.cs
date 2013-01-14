using System;
using System.Linq.Expressions;
using System.Web.Script.Serialization;
using CC.Core.Html;
using MethodFitness.Core.Enumerations;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Services.ViewOptions
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