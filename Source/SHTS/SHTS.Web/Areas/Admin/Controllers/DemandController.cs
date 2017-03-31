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
    public class DemandController : AdminBaseController
    {
        protected const string USERINFO = "userinfo";
        DemandManager demandManager = new DemandManager();
        DemandService demandService = new DemandService();

        [Permission(EnumRole.Normal)]
        public ActionResult Index(string id, string resourceType)
        {
            DemandModel model = new DemandModel();

            //页码，总数重置
            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }
            int allCount = 0;
            //model.Demands = demandService.GetDemands(20, page, out allCount);//每页显示20条

            DemandParameters parameters = new DemandParameters();
            parameters.PageCount = 20;
            parameters.PageIndex = page;
            parameters.ResourceType = resourceType;

            model.Demands = demandService.GetDemandsByParameters(parameters, out allCount);

            model.ResourceType = resourceType;
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
            //model.DemandCategories = demandManager.GetDemandCategories();
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
        public ActionResult Delete(string demandIdString)
        {
            var demandIdArray = (demandIdString ?? string.Empty).Split(',');
            var demandIdList = new List<int>();
            var tempDemandId = -1;

            foreach (var idString in demandIdArray)
            {
                if (int.TryParse(idString, out tempDemandId))
                {
                    demandIdList.Add(tempDemandId);
                }
            }

            foreach (var demandId in demandIdList)
            {
                Demand demand = demandService.GetDemandById(demandId);
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

            var jsonData = new
            {
                IsSuccessful = true
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
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

        public ActionResult UpdateDemandStatusAsComplete(string demandIdString)
        {
            var demandIdArray = (demandIdString ?? string.Empty).Split(',');
            var demandIdList = new List<int>();
            var tempDemandId = -1;

            foreach (var idString in demandIdArray)
            {
                if (int.TryParse(idString, out tempDemandId))
                {
                    demandIdList.Add(tempDemandId);
                }
            }

            foreach (var demandId in demandIdList)
            {
                demandService.UpdateDemandStatus(demandId, DemandStatus.Complete);
            }

            var jsonData = new
            {
                IsSuccessful = true
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}
