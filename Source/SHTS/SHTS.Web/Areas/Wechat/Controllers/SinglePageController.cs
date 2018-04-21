using System;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Models;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class SinglePageController : WechatBaseController
    {
        //
        // GET: /Wechat/Error/

        SinglePageService singlePageService = new SinglePageService();

        public ActionResult Show(string id)
        {
            NewsModel model = new NewsModel();

            int tempId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out tempId);
            }

            if (tempId != 0)
            {
                model.News = singlePageService.GetSingPageById(id);
            }

            if (model.News == null)
            {
                model.News = new SinglePage();
                model.News.Title = "该新闻不存在或已被删除";
                model.News.InsertTime = DateTime.Now;
            }

            return View(model);
        }

    }
}
