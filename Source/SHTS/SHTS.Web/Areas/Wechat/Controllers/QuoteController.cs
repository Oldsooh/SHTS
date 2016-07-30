using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class QuoteController : Controller
    {
        //
        // GET: /Wechat/Qoute/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 我收到的需求报价列表
        /// </summary>
        /// <returns></returns>
        public ActionResult RecievedQuotes()
        {
            return View();
        }

        /// <summary>
        /// 我发出的需求报价列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PostedQuotes()
        {
            return View();
        }
    }
}
