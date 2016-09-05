using System.Web.Mvc;

namespace Witbird.SHTS.Web.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Mobile";
            }
        }


        public override void RegisterArea(AreaRegistrationContext context)
        {
            //Detail页面
            context.MapRoute(
                name: "mobile_Detail",
                url: "Mobile/{controller}/{id}.html",
                defaults: new { action = "Show", id = "0" },
                constraints: new { controller = @"About|News|Demand|Trade|Activity|Resource", id = @"[\d]{0,5}" },
                namespaces: new string[] { "Witbird.SHTS.Web.Areas.Mobile.Controllers" }
            );
            
            context.MapRoute(
                "Mobile_default",
                "Mobile/{controller}/{action}/{id}",
                new { controller = "Index", action = "Index", id = UrlParameter.Optional },
                constraints: null,
                namespaces: new string[] { "Witbird.SHTS.Web.Areas.Mobile.Controllers" }
            );
        }
    }
}
