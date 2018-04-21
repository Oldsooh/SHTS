using System;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class NewsController : AdminBaseController
    {
        SinglePageManager singlePageManager = new SinglePageManager();
        SinglePageService singlePageService = new SinglePageService();

        public ActionResult Index(string id)
        {
            SinglePageModel model = new SinglePageModel();

            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }

            try
            {
                int newsCount = 0;
                model.SinglePages = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), null, true, 20, page, out newsCount);
                //分页
                if (model.SinglePages != null && model.SinglePages.Count > 0)
                {
                    model.PageIndex = page;//当前页数
                    model.PageSize = 20;//每页显示多少条
                    model.PageStep = 10;//每页显示多少页码
                    model.AllCount = newsCount;
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

        [HttpGet]
        public ActionResult Edit(string id)
        {
            SinglePageModel model = new SinglePageModel();
            model.SinglePage = singlePageService.GetSingPageById(id);
            if (model.SinglePage == null)
            {
                model.SinglePage = new SinglePage();
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Permission(EnumRole.Editer)]
        public ActionResult Edit(string id, string title, string keywords, string description, string category, string imageURl, string contentStyle)
        {
            string result = "更新失败";

            if (!string.IsNullOrEmpty(id) &&
                !string.IsNullOrEmpty(title) &&
                !string.IsNullOrEmpty(contentStyle))
            {
                SinglePage singlePage = singlePageService.GetSingPageById(id);
                if (singlePage != null)
                {
                    singlePage.Title = title;
                    singlePage.Keywords = keywords;
                    singlePage.Description = description;
                    singlePage.ImageUrl = imageURl;
                    singlePage.ContentStyle = contentStyle;
                    singlePage.ContentText = Witbird.SHTS.Common.Html.HtmlUtil.RemoveHTMLTags(singlePage.ContentStyle);
                    if (singlePageService.EditSinglePage(singlePage))
                    {
                        result = "success";
                    }
                }
            }
            else
            {
                result = "必填项不能为空";
            }

            return Content(result);
        }


        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        [Permission(EnumRole.Editer)]
        public ActionResult Add(string title, string keywords, string description, string category, string imageURl, string contentStyle)
        {
            string result = "添加失败";

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(contentStyle))
            {
                SinglePage singlePage = new SinglePage();
                singlePage.EntityType = EnumEntityType.News.ToString();
                singlePage.Category = category;
                singlePage.Title = title;
                singlePage.Keywords = keywords;
                singlePage.Description = description;
                singlePage.ContentStyle = contentStyle;
                singlePage.ContentText = Witbird.SHTS.Common.Html.HtmlUtil.RemoveHTMLTags(singlePage.ContentStyle);
                singlePage.ImageUrl = imageURl;
                singlePage.IsActive = true;
                singlePage.ViewCount = 0;
                singlePage.InsertTime = DateTime.Now;
                if (singlePageManager.AddSinglePage(singlePage))
                {
                    result = "success";
                }
            }
            else
            {
                result = "必填项不能为空";
            }

            return Content(result);
        }

        [HttpGet]
        [Permission(EnumRole.Editer)]
        public ActionResult Delete(int id)
        {
            singlePageService.DeleteSinglePageById(id);

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

    }
}
