using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class AboutController : AdminBaseController
    {
        SinglePageManager singlePageManager = new SinglePageManager();
        SinglePageService singlePageService = new SinglePageService();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Permission(EnumRole.Editer)]
        public ActionResult Edit(string id)
        {
            SinglePageModel model = new SinglePageModel();
            model.SinglePage = singlePageService.GetSingPageById(id);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Permission(EnumRole.Editer)]
        public ActionResult Edit(string id, string title, string contentStyle)
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
                    singlePage.ContentStyle = contentStyle;
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
    }
}
