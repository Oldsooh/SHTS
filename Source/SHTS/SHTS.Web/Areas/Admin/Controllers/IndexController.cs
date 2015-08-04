using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Web.Areas.Admin.Authorize;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    [Permission(EnumRole.Normal,EnumBusinessPermission.None)]
    public class IndexController : AdminBaseController
    {
        //
        // GET: /Admin/Index/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Top()
        {
            return View(UserInfo);
        }

        public ActionResult Left()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }
    }
}
