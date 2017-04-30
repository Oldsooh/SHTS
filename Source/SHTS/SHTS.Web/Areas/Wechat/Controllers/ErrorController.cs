using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Web.Areas.Wechat.Models;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class ErrorController : WechatBaseController
    {
        //
        // GET: /Wechat/Error/

        public ActionResult Index(ErrorMessage errorMessage)
        {
            return View(errorMessage);
        }

        public ActionResult Error_503()
        {
            return View();
        }

    }
}
