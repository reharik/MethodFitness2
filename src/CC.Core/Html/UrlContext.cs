using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CC.Core.Enumerations;
using CC.Core.Utilities;

namespace CC.Core.Html
{
    public static class UrlContext
    {
        static UrlContext()
        {
            Reset();
        }

        public static void Reset()
        {
            if (HttpRuntime.AppDomainAppVirtualPath != null)
            {
                Live();
                return;
            }

            Stub("");
        }

        public static void Stub()
        {
            Stub("");
        }

        public static void Stub(string usingFakeUrl)
        {
            Combine = (basePath, subPath) => "{0}/{1}".ToFormat(basePath.TrimEnd('/'), subPath.TrimStart('/'));
            ToAbsolute = path => Combine(usingFakeUrl, path.Replace("~", ""));
            ToFull = path => Combine(usingFakeUrl, path.Replace("~", ""));
            ToPhysicalPath = virtPath => virtPath.Replace("~", "").Replace("/", "\\");
        }

        public static void Live()
        {
            Combine = VirtualPathUtility.Combine;
            ToAbsolute = path =>
            {
                var result = path.Replace("~", VirtualPathUtility.ToAbsolute("~"));
                return result.StartsWith("//") ? result.Substring(1) : result;
            };
            ToFull = path =>
            {
                var baseUri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
                return new Uri(baseUri, ToAbsolute(path)).ToString();
            };
            ToPhysicalPath = HttpContext.Current.Server.MapPath;
        }

        public static Func<string, string, string> Combine { get; private set; }
        public static Func<string, string> ToAbsolute { get; private set; }
        public static Func<string, string> ToFull { get; private set; }
        public static Func<string, string> ToPhysicalPath { get; private set; }

        public static string GetUrl(string url)
        {
            return ToAbsolute(url.StartsWith("~")?url: Combine("~/", url));
        }

        public static string GetFullUrl(string path)
        {
            return ToFull(path);
        }

        public static string GetUrlForAction<CONTROLLER>(string action, AreaName areaName = null) where CONTROLLER : Controller
        {
            string controllerName = typeof (CONTROLLER).Name.Replace("Controller", "");
            string _area = areaName != null ? areaName.Key + "/" : "/";
            return ToAbsolute(Combine("~/", _area + controllerName + "/" + action));
        }

        public static string GetUrlForAction<CONTROLLER>(Expression<Func<CONTROLLER, object>> expression, AreaName areaName = null)
        {
            string controllerName = typeof(CONTROLLER).Name.Replace("Controller", "");
            string action = ReflectionHelper.GetMethod(expression).Name;
            string _area = areaName!=null? areaName.Key + "/" : "/";
            return ToAbsolute(Combine("~/", _area + controllerName + "/" + action));
        }

        public static string GetUrlForAction(string controllerName, string action, AreaName areaName = null)
        {
            var area = areaName != null ? areaName.Key + "/" : "/";
            return ToAbsolute(Combine("~/", area + controllerName + "/" + action));
        }
    }
}