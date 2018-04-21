using System.Web.Mvc;

namespace Witbird.SHTS.Web.Areas.Mobile.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Wechat/Error/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error_503()
        {
            return View();
        }

    }
}
