using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Models;
using Witbird.SHTS.Web.Controllers;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class ResourceController : WechatBaseController
    {
        ResourceManager resourceManager = new ResourceManager();
        ResourceService resourceService = new ResourceService();

        public static string UserIdentifyUrl =
            "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain
            + "/WeChat/User/Identify?returnUrl={0}\">认证会员可见</a>";

        #region 查看资源信息列表

        /// <summary>
        /// 页码从1开始
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("spacelist")]
        public ActionResult SpaceList()
        {
            var query = new Witbird.SHTS.Model.Extensions.QueryResource();
            query.ResourceType = 1;

            var filters = ProcessSpaceFilters();
            query.CityId = GetCityFilter();
            query.AreaId = GetFilterValue(filters, "ara");
            query.PageIndex = GetIntFilterValue(filters, "page");
            query.SpaceType = GetIntFilterValue(filters, "sptp");
            query.SpaceFeature = GetIntFilterValue(filters, "spft");
            query.SpaceFacility = (int)Math.Pow(2, GetIntFilterValue(filters, "spfc") - 1);
            query.SpaceSizeId = GetIntFilterValue(filters, "spsz");
            query.SpacePeopleId = GetIntFilterValue(filters, "sppc");
            query.SpaceTreat = GetIntFilterValue(filters, "sptt");
            query.PageIndex = query.PageIndex > 1 ? query.PageIndex - 1 : 0;
            query.PageSize = 15;
            query.State = 2;

            var result = resourceService.GetResourceByFilter(query);

            result.Filter = new Witbird.SHTS.Model.Extensions.UserFilter { ActionName = "spacelist", SelectedFilter = new Dictionary<string, string>(), UnselectFilter = new Dictionary<string, string>() };
            result.Paging.SelectedFilters = new Dictionary<string, string>();
            foreach (var item in filters)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    result.Filter.UnselectFilter.Add(item.Key, item.Value);
                }
                else
                {
                    result.Filter.SelectedFilter.Add(item.Key, item.Value);
                    result.Paging.SelectedFilters.Add(item.Key, item.Value);
                }
            }
            if (!string.IsNullOrEmpty(CurrentCityId))
            {
                if (result.Filter.SelectedFilter.ContainsKey("city"))
                {
                    result.Filter.SelectedFilter.Remove("city");
                }
                result.Filter.SelectedFilter.Add("city", CurrentCityId);
            }


            return View("ResourceListView", result);
        }

        /// <summary>
        /// 页码从1开始
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("actorlist")]
        public ActionResult ActorList()
        {
            var query = new Witbird.SHTS.Model.Extensions.QueryResource();
            query.ResourceType = 2;

            var filters = ProcessActorFilters();
            query.CityId = GetCityFilter();
            query.AreaId = GetFilterValue(filters, "ara");
            query.ActorFromId = GetIntFilterValue(filters, "acfm");
            query.ActorSex = GetIntFilterValue(filters, "acsx");
            query.ActorTypeId = GetIntFilterValue(filters, "actp");
            query.PageIndex = GetIntFilterValue(filters, "page");
            query.PageIndex = query.PageIndex > 1 ? query.PageIndex - 1 : 0;
            query.PageSize = 15;
            query.State = 2;

            var result = resourceService.GetResourceByFilter(query);

            result.Filter = new Witbird.SHTS.Model.Extensions.UserFilter { ActionName = "actorlist", SelectedFilter = new Dictionary<string, string>(), UnselectFilter = new Dictionary<string, string>() };
            result.Paging.SelectedFilters = new Dictionary<string, string>();
            foreach (var item in filters)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    result.Filter.UnselectFilter.Add(item.Key, item.Value);
                }
                else
                {
                    result.Filter.SelectedFilter.Add(item.Key, item.Value);
                    result.Paging.SelectedFilters.Add(item.Key, item.Value);
                }
            }
            if (!string.IsNullOrEmpty(CurrentCityId))
            {
                if (result.Filter.SelectedFilter.ContainsKey("city"))
                {
                    result.Filter.SelectedFilter.Remove("city");
                }
                result.Filter.SelectedFilter.Add("city", CurrentCityId);
            }

            return View("ResourceListView", result);
        }

        /// <summary>
        /// 页码从1开始
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("equipmentlist")]
        public ActionResult EquipmentList()
        {
            var query = new Witbird.SHTS.Model.Extensions.QueryResource();
            query.ResourceType = 3;

            var filters = ProcessEquipmentFilters();
            query.CityId = GetCityFilter();
            query.AreaId = GetFilterValue(filters, "ara");
            query.PageIndex = GetIntFilterValue(filters, "page");
            query.EquipTypeId = GetIntFilterValue(filters, "eqtp");

            query.PageIndex = query.PageIndex > 1 ? query.PageIndex - 1 : 0;

            query.PageSize = 15;
            query.State = 2;

            var result = resourceService.GetResourceByFilter(query);

            result.Filter = new Witbird.SHTS.Model.Extensions.UserFilter { ActionName = "equipmentlist", SelectedFilter = new Dictionary<string, string>(), UnselectFilter = new Dictionary<string, string>() };
            result.Paging.SelectedFilters = new Dictionary<string, string>();
            foreach (var item in filters)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    result.Filter.UnselectFilter.Add(item.Key, item.Value);
                }
                else
                {
                    result.Filter.SelectedFilter.Add(item.Key, item.Value);
                    result.Paging.SelectedFilters.Add(item.Key, item.Value);
                }
            }
            if (!string.IsNullOrEmpty(CurrentCityId))
            {
                if (result.Filter.SelectedFilter.ContainsKey("city"))
                {
                    result.Filter.SelectedFilter.Remove("city");
                }
                result.Filter.SelectedFilter.Add("city", CurrentCityId);
            }

            return View("ResourceListView", result);
        }

        /// <summary>
        /// 页码从1开始
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("otherlist")]
        public ActionResult OtherList()
        {
            var query = new Witbird.SHTS.Model.Extensions.QueryResource();
            query.ResourceType = 4;

            var filters = ProcessOtherFilters();
            query.CityId = GetCityFilter();
            query.AreaId = GetFilterValue(filters, "ara");
            query.PageIndex = GetIntFilterValue(filters, "page");
            query.OtherTypeId = GetIntFilterValue(filters, "ottp");
            query.PageIndex = query.PageIndex > 1 ? query.PageIndex - 1 : 0;
            query.PageSize = 15;
            query.State = 2;

            var result = resourceService.GetResourceByFilter(query);

            result.Filter = new Witbird.SHTS.Model.Extensions.UserFilter { ActionName = "otherlist", SelectedFilter = new Dictionary<string, string>(), UnselectFilter = new Dictionary<string, string>() };
            result.Paging.SelectedFilters = new Dictionary<string, string>();
            foreach (var item in filters)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    result.Filter.UnselectFilter.Add(item.Key, item.Value);
                }
                else
                {
                    result.Filter.SelectedFilter.Add(item.Key, item.Value);
                    result.Paging.SelectedFilters.Add(item.Key, item.Value);
                }
            }
            if (!string.IsNullOrEmpty(CurrentCityId))
            {
                if (result.Filter.SelectedFilter.ContainsKey("city"))
                {
                    result.Filter.SelectedFilter.Remove("city");
                }
                result.Filter.SelectedFilter.Add("city", CurrentCityId);
            }

            return View("ResourceListView", result);
        }
        #endregion

        #region 详情页
        [ActionName("show")]
        public ActionResult Show(int id)
        {
            var space = resourceManager.GetResourceById(id);
            if (space == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (!IsIdentified)
                {
                    string url = string.Format(UserIdentifyUrl, Request.Url.AbsoluteUri);
                    space.Telephone = url;
                    space.Mobile = url;
                    space.QQ = url;
                    space.WeChat = string.Empty;
                    space.Email = url;
                    space.Contract = url;
                    space.DetailAddress = url;
                    space.Href = url;
                }

                try
                {
                    CommonService commonService = new CommonService();
                    ViewData["RightModel"] = commonService.GetRight();
                }
                catch (Exception ex)
                {
                    LogService.Log("resource Show GetRight", ex.ToString());
                }

                ViewData["CurrentWeChatUser"] = CurrentWeChatUser;
                return View(space);
            }
        }


        #endregion

        [ActionName("create")]
        public ActionResult Create()
        {
            if (!IsIdentified)
            {
                return Redirect("/wechat/user/identify");
            }
            else
            {
                WeChatResourceViewModel model = new WeChatResourceViewModel();
                return View("Create", model);
            }
        }

        [HttpPost]
        [ActionName("create")]
        [ValidateInput(false)]
        public ActionResult CreateResource(WeChatResourceViewModel model)
        {
            bool isSuccessful = false;
            try
            {
                Witbird.SHTS.DAL.New.Resource resource = new Witbird.SHTS.DAL.New.Resource();

                resource.ResourceType = int.Parse(model.ResType);

                resource.UserId = UserInfo.UserId;

                resource.Title = model.Title;
                resource.CanFriendlyLink = model.CanFriendlyLink;
                resource.ProvinceId = model.ddlProvince;
                resource.CityId = model.ddlCity;
                resource.AreaId = model.ddlArea;
                resource.CreateTime = DateTime.Now;
                resource.SpaceSizeId = string.IsNullOrEmpty(model.SpaceSizeId) ? 1 : int.Parse(model.SpaceSizeId);
                resource.ShortDesc = model.Description == null ? null : Witbird.SHTS.Common.Html.HtmlUtil.RemoveHTMLTags(model.Description);
                resource.Description = model.Description;
                resource.SpacePeopleId = string.IsNullOrEmpty(model.SpacePeopleId) ? 1 : int.Parse(model.SpacePeopleId);
                resource.SpaceTreat = int.Parse(model.SpaceTreat);
                resource.Contract = model.Contact;
                resource.Email = model.Email;
                resource.DetailAddress = model.DetailAddress;
                resource.QQ = model.QQ;
                resource.Telephone = model.Telephone;
                resource.Mobile = model.Mobile;
                resource.WeChat = model.WeChat;
                resource.Href = model.Href;
                resource.ImageUrls = model.ImageUrls;
                resource.LastUpdatedTime = DateTime.Now;
                resource.State = (int)ResourceState.Created;
                resource.ClickCount = 0;
                resource.ClickTime = DateTime.Now;
                resource.UserName = UserInfo.UserName;

                resource.ActorFromId = string.IsNullOrEmpty(model.ActorFromId) ? 1 : int.Parse(model.ActorFromId);
                resource.ActorSex = string.IsNullOrEmpty(model.ActorSex) ? 1 : int.Parse(model.ActorSex);

                resource.SpaceTypeId = string.IsNullOrEmpty(model.SpaceTypeId) ? 0 : int.Parse(model.SpaceTypeId);
                resource.ActorTypeId = string.IsNullOrEmpty(model.ActorTypeId) ? 0 : int.Parse(model.ActorTypeId);
                resource.EquipTypeId = string.IsNullOrEmpty(model.EquipTypeId) ? 0 : int.Parse(model.EquipTypeId);
                resource.OtherTypeId = string.IsNullOrEmpty(model.OtherTypeId) ? 0 : int.Parse(model.OtherTypeId);

                if (!string.IsNullOrEmpty(model.SpaceFacilities))
                {
                    int facilityvalue = 0;
                    var facilities = model.SpaceFacilities.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().Select(v => int.Parse(v)).ToList();
                    foreach (var item in facilities)
                    {
                        facilityvalue |= (int)Math.Pow(2, item - 1);
                    }
                    resource.SpaceFacilityValue = facilityvalue;
                }

                if (!string.IsNullOrEmpty(model.SpaceFeatures))
                {
                    int featurevalue = 0;
                    var features = model.SpaceFeatures.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().Select(v => int.Parse(v)).ToList();
                    foreach (var item in features)
                    {
                        featurevalue |= (int)Math.Pow(2, item - 1);
                    }
                    resource.SpaceFeatureValue = featurevalue;
                }

                if (resource.ShortDesc != null && resource.ShortDesc.Length > 150)
                {
                    resource.ShortDesc = resource.ShortDesc.Substring(0, 147) + "...";
                }

                resourceManager.CreateResource(resource);
                isSuccessful = true;

            }
            catch (Exception ex)
            {
                isSuccessful = false;
                LogService.LogWexin("CreateResource", ex.ToString());
            }

            var data = new
            {
                IsSuccessful = isSuccessful
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region Process Filters
        private string GetFilterValue(Dictionary<string, string> filterDict, string filterName)
        {
            string value = string.Empty;
            if (filterDict != null && !string.IsNullOrEmpty(filterName))
            {
                if (filterDict.Keys.Contains(filterName))
                {
                    value = filterDict[filterName];
                }
            }
            return value;
        }

        private int GetIntFilterValue(Dictionary<string, string> filterDict, string filterName)
        {
            int value = 0;
            if (filterDict != null && !string.IsNullOrEmpty(filterName)
                && filterDict.Keys.Contains(filterName) && int.TryParse(filterDict[filterName], out value))
            {
            }
            return value;
        }

        private Dictionary<string, string> ProcessSpaceFilters()
        {
            string[] filters = new[] { "ara", "city", "page", "sptp", "spft", "spfc", "spsz", "sppc", "sptt", "rsst", "rsed" };//城市,分页,场地类型,场地特点,场地设施,场地大小,容纳人数,提供酒宴,开始时间,结束时间

            return ProcessFiltersCore(filters);
        }

        private Dictionary<string, string> ProcessActorFilters()
        {
            string[] filters = new[] { "ara", "city", "page", "actp", "rsst", "rsed", "acfm", "acsx" };//城市,分页,演员工作,开始时间,结束时间,来源,性别

            return ProcessFiltersCore(filters);
        }

        private Dictionary<string, string> ProcessEquipmentFilters()
        {
            string[] filters = new[] { "ara", "city", "page", "eqtp", "rsst", "rsed" };//城市,分页,开始时间,结束时间

            return ProcessFiltersCore(filters);
        }

        private Dictionary<string, string> ProcessOtherFilters()
        {
            string[] filters = new[] { "ara", "city", "page", "ottp", "rsst", "rsed" };//城市,分页,开始时间,结束时间

            return ProcessFiltersCore(filters);
        }

        private Dictionary<string, string> ProcessFiltersCore(params string[] filters)
        {
            Dictionary<string, string> filterDict = new Dictionary<string, string>();

            foreach (var filter in filters)
            {
                var value = ProcessFilter(filter);
                filterDict.Add(filter, value);
            }

            return filterDict;
        }

        private string GetCityFilter()
        {
            string city = null;
            if (!string.IsNullOrEmpty(CurrentCityId))
            {
                city = CurrentCityId;
            }
            return city;
        }

        private string ProcessFilter(string filterName)
        {
            if (string.IsNullOrEmpty(filterName) || string.IsNullOrEmpty(Request.QueryString[filterName]))
            {
                return string.Empty;
            }
            else
            {
                return Request.QueryString[filterName];
            }
        }

        #endregion

    }
}
