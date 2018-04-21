using System.Web.Mvc;
using System.Web.Routing;

namespace Witbird.SHTS.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Detail页面
            routes.MapRoute(
                name: "Detail",
                url: "{controller}/{id}.html",
                defaults: new { action = "Show", id = "0" },
                constraints: new { controller = @"About|News|Demand|Trade|Activity|Resource", id = @"[\d]{0,5}" },
                namespaces: new string[] { "Witbird.SHTS.Web.Controllers" }
            );

            //搜索页面
            routes.MapRoute(
                name: "Search",
                url: "Search/{keyWords}/{page}",
                defaults: new { controller = "Search", action = "Index", keyWords = UrlParameter.Optional },
                namespaces: new string[] { "Witbird.SHTS.Web.Controllers" }
            );

            //默认
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: null,
                namespaces: new string[] { "Witbird.SHTS.Web.Controllers" }
            );
        }
    }
}