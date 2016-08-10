using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
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
