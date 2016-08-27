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
