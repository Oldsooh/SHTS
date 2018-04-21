using System;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Models;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class NewsController : WechatBaseController
    {

        SinglePageService singlePageService = new SinglePageService();

        public ActionResult Index(string id)
        {
            NewsModel model = new NewsModel();

            try
            {
                //页码，总数重置
                int page = 1;
                if (!string.IsNullOrEmpty(id))
                {
                    Int32.TryParse(id, out page);
                }
                int allCount = 0;

                //新闻分页列表,第页10条
                model.Newses = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), null, true, 10, page, out allCount);
                //分页
                if (model.Newses != null && model.Newses.Count > 0)
                {
                    model.PageIndex = page;//当前页数
                    model.PageSize = 10;//每页显示多少条
                    model.PageStep = 5;//每页显示多少页码
                    model.AllCount = allCount;//总条数
                    if (model.AllCount % model.PageSize == 0)
                    {
                        model.PageCount = model.AllCount / model.PageSize;
                    }
                    else
                    {
                        model.PageCount = model.AllCount / model.PageSize + 1;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("新闻列表", e.ToString());
            }

            return View(model);
        }


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
