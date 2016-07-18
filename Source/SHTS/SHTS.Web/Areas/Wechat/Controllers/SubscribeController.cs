using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class SubscribeController : WechatBaseController
    {
        //
        // GET: /Wechat/Subscribe/

        public ActionResult Index()
        {
            return View();
        }

    }
}
