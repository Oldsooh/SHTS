using System.Web.Mvc;

namespace WitBird.SHTS.Areas.WeChatAuth
{
    public class WeChatAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WeChatAuth";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WeChatAuth_default",
                "WeChatAuth/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
