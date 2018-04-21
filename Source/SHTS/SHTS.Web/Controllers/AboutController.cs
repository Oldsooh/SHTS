using System;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Controllers
{
    public class AboutController : Controller
    {
        SinglePageManager singlePageManager = new SinglePageManager();
        SinglePageService singlePageService = new SinglePageService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show(string id)
        {
            int newsId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out newsId);
            }
            SinglePageModel model = new SinglePageModel();
            model.SinglePage = singlePageService.GetSingPageById(id.ToString());
            if (model.SinglePage == null)
            {
                model.SinglePage = new SinglePage();
                model.SinglePage.Title = "该记录不存在或已被删除";
            }
            return View(model);
        }
    }
}
