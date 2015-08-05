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
