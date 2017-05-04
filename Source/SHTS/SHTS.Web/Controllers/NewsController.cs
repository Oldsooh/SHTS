using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Controllers
{
    public class NewsController : PCBaseController
    {
        SinglePageManager singlePageManager = new SinglePageManager();
        SinglePageService singlePageService = new SinglePageService();

        //新闻首页
        public ActionResult Index(string id)
        {
            NewsModel model = new NewsModel();

            try
            {
                int newsCount = 0;
                model.Slides = singlePageService.GetSlides();
                model.Notices = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Notice.ToString(), true, 6, 1, out newsCount);
                model.Companys = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Company.ToString(), true, 6, 1, out newsCount);
                model.Industrys = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Industry.ToString(), true, 6, 1, out newsCount);
                model.Resources = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Resource.ToString(), true, 6, 1, out newsCount);
                model.Supplydemands = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Supplydemand.ToString(), true, 6, 1, out newsCount);

                //页码，总数重置
                int page = 1;
                if (!string.IsNullOrEmpty(id))
                {
                    Int32.TryParse(id, out page);
                }
                int allCount = 0;

                //新闻分页列表,第页30条
                model.Newses = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), null, true, 30, page, out allCount);
                //分页
                if (model.Newses != null && model.Newses.Count > 0)
                {
                    model.PageIndex = page;//当前页数
                    model.PageSize = 30;//每页显示多少条
                    model.PageStep = 10;//每页显示多少页码
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

            if (model.Slides == null)
            {
                model.Slides = new List<SinglePage>();
            }
            if (model.Notices == null)
            {
                model.Notices = new List<SinglePage>();
            }
            if (model.Companys == null)
            {
                model.Companys = new List<SinglePage>();
            }
            if (model.Industrys == null)
            {
                model.Industrys = new List<SinglePage>();
            }
            if (model.Resources == null)
            {
                model.Resources = new List<SinglePage>();
            }
            if (model.Supplydemands == null)
            {
                model.Supplydemands = new List<SinglePage>();
            }

            return View(model);
        }

        public ActionResult Show(string id)
        {
            NewsModel model = new NewsModel();

            int newsId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out newsId);
            }
            
            try
            {
                SinglePage news = singlePageService.GetSingPageById(id.ToString());
                model.News = news;
                int newsCount = 0;
                model.Slides = singlePageService.GetSlides();
                model.Notices = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Notice.ToString(), true, 6, 1, out newsCount);
                model.Companys = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Company.ToString(), true, 6, 1, out newsCount);
                model.Industrys = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Industry.ToString(), true, 6, 1, out newsCount);
                model.Resources = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Resource.ToString(), true, 6, 1, out newsCount);
                model.Supplydemands = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), EnumNews.Supplydemand.ToString(), true, 6, 1, out newsCount);
            }
            catch (Exception e)
            {
                LogService.Log("新闻列表", e.ToString());
            }

            if (model.News == null)
            {
                model.News = new SinglePage();
                model.News.Title = "该记录不存在或已被删除";
            }
            if (model.Slides == null)
            {
                model.Slides = new List<SinglePage>();
            }
            if (model.Notices == null)
            {
                model.Notices = new List<SinglePage>();
            }
            if (model.Companys == null)
            {
                model.Companys = new List<SinglePage>();
            }
            if (model.Industrys == null)
            {
                model.Industrys = new List<SinglePage>();
            }
            if (model.Resources == null)
            {
                model.Resources = new List<SinglePage>();
            }
            if (model.Supplydemands == null)
            {
                model.Supplydemands = new List<SinglePage>();
            }

            return View(model);
        }
    }
}
