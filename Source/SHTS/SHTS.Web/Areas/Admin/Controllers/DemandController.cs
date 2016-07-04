using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class DemandController : Controller
    {
        protected const string USERINFO = "userinfo";
        DemandManager demandManager = new DemandManager();
        DemandService demandService = new DemandService();

        [Permission(EnumRole.Editer)]
        public ActionResult Index(string id)
        {
            DemandModel model = new DemandModel();
            model.DemandCategories = demandManager.GetDemandCategories();

            //页码，总数重置
            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }
            int allCount = 0;
            model.Demands = demandService.GetDemands(20, page, out allCount);//每页显示20条
            //分页
            if (model.Demands != null && model.Demands.Count > 0)
            {
                model.PageIndex = page;//当前页数
                model.PageSize = 20;//每页显示多少条
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

            return View(model);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult Category()
        {
            DemandModel model = new DemandModel();
            model.DemandCategories = demandManager.GetDemandCategories();
            return View(model);
        }

        [HttpGet]
        [Permission(EnumRole.Editer)]
        public ActionResult SensitiveWords(string id)
        {
            SinglePageModel model = new SinglePageModel();
            SinglePageService singlePageService = new SinglePageService();
            model.SinglePage = singlePageService.GetSingPageById(id);
            return View(model);
        }

        [HttpPost]
        [Permission(EnumRole.Editer)]
        public ActionResult SensitiveWords(string id, string contentStyle)
        {
            string result = "更新失败";

            if (!string.IsNullOrEmpty(id) &&
                !string.IsNullOrEmpty(contentStyle))
            {
                SinglePageService singlePageService = new SinglePageService();
                SinglePage singlePage = singlePageService.GetSingPageById(id);
                if (singlePage != null)
                {
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

        [HttpPost]
        [Permission(EnumRole.Editer)]
        public ActionResult AddCategory(string name, string displayOrder)
        {
            int tempDisplayOrder = 0;
            if (!string.IsNullOrEmpty(name) && Int32.TryParse(displayOrder, out tempDisplayOrder))
            {
                DemandCategory demandCategory = new DemandCategory();
                demandCategory.Name = name;
                demandCategory.DisplayOrder = tempDisplayOrder;
                demandCategory.IsActive = true;
                demandManager.AddCategory(demandCategory);
            }
            return Redirect("/admin/demand/category");
        }

        [HttpPost]
        [Permission(EnumRole.Editer)]
        public ActionResult EditCategory(string id, string name, string displayOrder)
        {
            int tempDisplayOrder = 0;
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name) && Int32.TryParse(displayOrder, out tempDisplayOrder))
            {
                DemandCategory demandCategory = demandManager.GetDemandCategoryById(Int32.Parse(id));
                if (demandCategory != null)
                {
                    demandCategory.Name = name;
                    demandCategory.DisplayOrder = tempDisplayOrder;
                    demandManager.EditCategory(demandCategory);
                }
            }
            return Redirect("/admin/demand/category");
        }

        [HttpGet]
        [Permission(EnumRole.Editer)]
        public ActionResult ActiveOrDeleteCategory(string id, string option)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(option))
            {
                DemandCategory demandCategory = demandManager.GetDemandCategoryById(Int32.Parse(id));
                if (demandCategory != null)
                {
                    if (option == "active")
                    {
                        demandCategory.IsActive = !demandCategory.IsActive;
                        demandManager.EditCategory(demandCategory);
                    }
                    if (option == "delete")
                    {
                        demandManager.DeleteCategory(demandCategory);
                    }
                }
            }
            return Redirect("/admin/demand/category");
        }

        [Permission(EnumRole.Editer)]
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Demand demand = demandService.GetDemandById(Int32.Parse(id));
                if (demand != null)
                {
                    demand.IsActive = false;
                    if (demand.Budget == null)
                    {
                        demand.Budget = 0;
                    }
                    demandService.EditDemand(demand);
                }
            }

            return Redirect(Request.UrlReferrer.LocalPath);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult UpdateDemandWeixinBuyFee(string demandIds, int weixinBuyFee)
        {
            bool result = false;
            List<int> ids = new List<int>();
            if (!string.IsNullOrEmpty(demandIds))
            {
                var idArray = demandIds.Split(',');
                foreach (var demandId in idArray)
                {
                    if (!string.IsNullOrEmpty(demandId))
                    {
                        ids.Add(Convert.ToInt32(demandId));
                    }
                }
            }
            result = demandService.UpdateWexinBuyFee(ids, weixinBuyFee);

            var jsonData = new {
                IsSuccessFul = result
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateDemandStatusAsComplete(int id)
        {
            if (id > -1)
            {
                demandService.UpdateDemandStatus(id, DemandStatus.Complete);
            }

            return Redirect(Request.UrlReferrer.LocalPath);
        }

        public ActionResult TestSendMessage()
        {
            var appId = "wx75defde278a65715";
            var secret = "429a0bbaacdd3c84bd4936f3efed6524";
            AccessTokenContainer.Register(appId, secret);
            //AccessTokenContainer.GetAccessToken();
            var log = "";
            try
            {
                var myOpenId = "o1HUAweo1dutYjcrfcNn1dxkC5zs";
                var timeout = 100000;
                var articles = new List<Article>()
                {
                    new Article()
                    {
                        Description = "Description",
                        PicUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/content/images/banner.jpg",
                        Title = "Test send message",
                        Url = "http://mp.weixin.qq.com/s?__biz=MzIzODAzMjg1Mg==&mid=406616045&idx=1&sn=0284c00c826b9faacc9fd51d61e90a31&scene=0&previewkey=hJ65r3CvPxZrCv2xPXuf8MNS9bJajjJKzz%2F0By7ITJA%3D#wechat_redirect"
                    }
                };

                var wxResult = CustomApi.SendNews(appId, myOpenId, articles, timeout);

                log += "ErrorCode: " + wxResult.errcode;
                log += "ErrorMsg: " + wxResult.errmsg;
            }
            catch (Exception ex)
            {
                log += "测试消息定向推送发生异常: " + ex.ToString();
            }

            return Content(log);
        }
    }
}
