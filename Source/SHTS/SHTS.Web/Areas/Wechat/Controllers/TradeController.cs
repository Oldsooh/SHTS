using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Models;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class TradeController : WechatBaseController
    {
        //
        // GET: /M/Trade/

        public ActionResult Index()
        {
            TradeModel model = new TradeModel();
            model.Trades = new List<Trade>(); //to do
            return View(model);
        }

    }
}
