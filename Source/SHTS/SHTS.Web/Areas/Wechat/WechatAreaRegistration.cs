using System.Web.Mvc;

namespace Witbird.SHTS.Web.Areas.Wechat
{
    public class WechatAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Wechat";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //Detail页面
            context.MapRoute(
                name: "Wechat_Detail",
                url: "Wechat/{controller}/{id}.html",
                defaults: new { action = "Show", id = "0" },
                constraints: new { controller = @"About|News|Demand|Trade|Activity|Resource", id = @"[\d]{0,5}" },
                namespaces: new string[] { "Witbird.SHTS.Web.Areas.Wechat.Controllers" }
            );

            context.MapRoute(
                "Wechat_default",
                "Wechat/{controller}/{action}/{id}",
                new { controller = "Index", action = "Index", id = UrlParameter.Optional },
                constraints: null,
                namespaces: new string[] { "Witbird.SHTS.Web.Areas.Wechat.Controllers" }
            );
        }
    }
}
