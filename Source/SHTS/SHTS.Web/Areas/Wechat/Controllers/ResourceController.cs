using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.ModelBinding;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Extensions;
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
            query.QuotePriceCondition = GetFilterValue(filters, "qpc");

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
            query.QuotePriceCondition = GetFilterValue(filters, "qpc");

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
            query.QuotePriceCondition = GetFilterValue(filters, "qpc");
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
            query.QuotePriceCondition = GetFilterValue(filters, "qpc");

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

        #region 创建资源
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
            string errorMessage = string.Empty;

            try
            {
                if (!ModelState.IsValid || model == null)
                {
                    isSuccessful = false;
                    errorMessage = "资源参数错误，请刷新页面后重试";
                }

                var resource = PrepareAndValidateResourceData(model);


                resourceManager.CreateResource(resource);
                isSuccessful = true;

            }
            catch (Exception ex)
            {
                isSuccessful = false;
                errorMessage = ex.Message;
                LogService.LogWexin("CreateResource", ex.ToString());
            }

            var data = new
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 编辑资源
        [ActionName("edit")]
        public ActionResult Edit(int id)
        {
            if (!IsIdentified)
            {
                return Redirect("/wechat/user/identify");
            }
            else
            {
                var model = resourceManager.GetResourceById(id);
                if (model == null || model.UserId != CurrentUser.UserId)
                {
                    return Redirect("/wechat/resource/my");
                }
                else
                {
                    WeChatResourceViewModel resource = new WeChatResourceViewModel();

                    //resource.CurrentUser = CurrentUser;
                    //resource.CurrentWechatUser = CurrentWeChatUser;
                    resource.ResId = model.Id.ToString();

                    resource.ResType = model.ResourceType.ToString();

                    resource.Title = model.Title;
                    resource.CanFriendlyLink = model.CanFriendlyLink;
                    resource.ddlProvince = model.ProvinceId;
                    resource.ddlCity = model.CityId;
                    resource.ddlArea = model.AreaId;

                    resource.SpaceSizeId = model.SpaceSizeId.ToString();
                    //resource.ShortDesc = model.Description == null ? null : Witbird.SHTS.Common.Html.HtmlUtil.RemoveHTMLTags(model.Description);
                    resource.Description = model.Description;
                    resource.SpacePeopleId = model.SpacePeopleId.ToString();
                    resource.SpaceTreat = model.SpaceTreat.ToString();
                    resource.Contact = model.Contract;
                    resource.Email = model.Email;
                    resource.DetailAddress = model.DetailAddress;

                    resource.QQ = model.QQ;
                    resource.Telephone = model.Telephone;
                    resource.Mobile = model.Mobile;
                    resource.WeChat = model.WeChat;
                    resource.Href = model.Href;
                    resource.ImageUrls = model.ImageUrls;
                    //resource.LastUpdatedTime = DateTime.Now;
                    //resource.State = (int)ResourceState.Created;
                    //resource.ClickCount = 0;
                    //resource.ClickTime = DateTime.Now;
                    resource.QuotePrice = model.QuotePrice ?? 0;

                    resource.SpaceTypeId = model.SpaceTypeId.ToString();
                    resource.ActorTypeId = model.ActorTypeId.ToString();
                    resource.EquipTypeId = model.EquipTypeId.ToString();
                    resource.OtherTypeId = model.OtherTypeId.ToString();

                    resource.ActorFromId = model.ActorFromId.ToString();
                    resource.ActorSex = model.ActorSex.ToString();

                    List<int> spacefacilities = new List<int>();
                    foreach (var item in Witbird.SHTS.Web.Public.MiscData.SpaceFacilityList)
                    {
                        if ((model.SpaceFacilityValue & (int)Math.Pow(2, item.Id - 1)) > 0)
                        {
                            spacefacilities.Add(item.Id);
                        }
                    }
                    resource.SpaceFacilities = string.Join(",", spacefacilities);

                    List<int> spacefeatures = new List<int>();
                    foreach (var item in Witbird.SHTS.Web.Public.MiscData.SpaceFeatureList)
                    {
                        if ((model.SpaceFeatureValue & (int)Math.Pow(2, item.Id - 1)) > 0)
                        {
                            spacefeatures.Add(item.Id);
                        }
                    }
                    resource.SpaceFeatures = string.Join(",", spacefeatures);

                    return View("Edit", resource);
                }
            }
        }

        [HttpPost]
        [ActionName("edit")]
        [ValidateInput(false)]
        public ActionResult Edit(WeChatResourceViewModel model)
        {
            var isSuccessful = false;
            var errorMessage = string.Empty;
            var resourceId = -1;

            try
            {

                if (!ModelState.IsValid || model == null)
                {
                    isSuccessful = false;
                    errorMessage = "资源参数错误，请刷新页面后重试";
                }
                int.TryParse(model.ResId, out resourceId);
                var originalResource = resourceManager.GetResourceById(resourceId);

                if (originalResource == null)
                {
                    isSuccessful = false;
                    errorMessage = "该资源记录不存在或已被删除";
                }
                else if (originalResource.UserId != UserInfo.UserId)
                {
                    isSuccessful = false;
                    errorMessage = "你只能编辑或删除自己发布的资源信息";
                }
                else
                {
                    var newResource = PrepareAndValidateResourceData(model);
                    newResource.Id = originalResource.Id;
                    newResource.CreateTime = originalResource.CreateTime;
                    newResource.ClickCount = originalResource.ClickCount;
                    newResource.ClickTime = originalResource.ClickTime;

                    resourceManager.EditResource(newResource);

                    isSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                errorMessage = ex.Message;
                LogService.LogWexin("编辑资源失败", ex.ToString());
            }

            var data = new
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult Delete(string id)
        {
            if (!IsUserLogin)
            {
                return Redirect("/wechat/account/login");
            }

            User user = UserInfo;

            if (!string.IsNullOrEmpty(id))
            {
                var resource = resourceManager.GetResourceById(Int32.Parse(id));
                if (resource != null && resource.UserId == user.UserId)
                {
                    resourceManager.DeleteResourceById(resource.Id);
                }
            }

            return Redirect(Request.UrlReferrer.LocalPath);
        }

        public ActionResult Click(string id)
        {
            if (!IsUserLogin)
            {
                return Redirect("/wechat/account/login");
            }

            User user = UserInfo;

            if (!string.IsNullOrEmpty(id))
            {
                var resource = resourceManager.GetResourceById(Int32.Parse(id));
                if (resource != null && resource.UserId == user.UserId)
                {
                    resourceManager.ClickResourceById(resource.Id);
                }
            }

            return Redirect(Request.UrlReferrer.LocalPath);
        }


        /// <summary>
        /// 我提交的资源
        /// </summary>
        /// <returns></returns>
        [ActionName("my")]
        public ActionResult My(int id = 1)
        {
            if (!IsUserLogin)
            {
                return Redirect("/account/login");
            }
            else
            {
                QueryResourceResult model = new QueryResourceResult();
                model.Paging.ActionName = "My";

                //页码，总数重置
                int pageIndex = 1;
                if (!string.IsNullOrEmpty(Request.QueryString["page"]))
                {
                    int.TryParse(Request.QueryString["page"], out pageIndex);
                }
                pageIndex = pageIndex < 1 ? 1 : pageIndex;

                model = resourceService.GetResourceByUser(UserInfo.UserId, pageIndex - 1, 15);

                //分页
                if (model.Items != null && model.Items.Count > 0)
                {
                    model.Paging.PageIndex = pageIndex;//当前页数
                    model.Paging.PageStep = 10;//每页显示多少页码
                    if (model.TotalCount % 15 == 0)
                    {
                        model.Paging.PageCount = model.TotalCount / 15;
                    }
                    else
                    {
                        model.Paging.PageCount = model.TotalCount / 15 + 1;
                    }
                }

                return View(model);
            }
        }

        private Witbird.SHTS.DAL.New.Resource PrepareAndValidateResourceData(WeChatResourceViewModel model)
        {
            Witbird.SHTS.DAL.New.Resource resource = new Witbird.SHTS.DAL.New.Resource();

            ParameterChecker.Check(model, "参数错误");

            var title = model.Title;
            var resourceTypeId = -1;
            var spaceTypeId = -1;
            var spaceSizeId = -1;
            var spacePeopleId = -1;
            var spaceTreat = -1;
            var actorTypeId = -1;
            var actorFromId = -1;
            var actorSexId = -1;
            var equipTypeId = -1;
            var otherTypeId = -1;
            var provinceId = model.ddlProvince;
            var areaId = model.ddlArea;
            var cityId = model.ddlCity;
            var contact = model.Contact;
            var detailAddress = model.DetailAddress;
            var telephone = model.Telephone;
            var mobilePhone = model.Mobile;
            var email = model.Email;
            var href = model.Href;
            var currentTime = DateTime.Now;
            var quotePrice = model.QuotePrice;

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("请填写资源标题");
            }

            if (!int.TryParse(model.ResType, out resourceTypeId))
            {
                throw new ArgumentException("资源类别参数错误，请刷新页面后重试");
            }

            if (resourceTypeId == 1)
            {
                if (!int.TryParse(model.SpaceTypeId, out spaceTypeId))
                {
                    throw new ArgumentException("场地类型参数错误，请刷新页面后重试");
                }

                if (!int.TryParse(model.SpaceSizeId, out spaceSizeId))
                {
                    throw new ArgumentException("场地面积参数错误，请刷新页面后重试");
                }

                if (!int.TryParse(model.SpacePeopleId, out spacePeopleId))
                {
                    throw new ArgumentException("场地容纳人数参数错误，请刷新页面后重试");
                }

                if (!int.TryParse(model.SpaceTreat, out spaceTreat))
                {
                    throw new ArgumentException("酒宴提供参数错误，请刷新页面后重试");
                }
            }
            else if (resourceTypeId == 2)
            {
                if (!int.TryParse(model.ActorFromId, out actorFromId))
                {
                    throw new ArgumentException("演员所属组织参数错误，请刷新页面后重试");
                }

                if (!int.TryParse(model.ActorTypeId, out actorTypeId))
                {
                    throw new ArgumentException("演员类型参数错误，请刷新页面后重试");
                }

                if (!int.TryParse(model.ActorSex, out actorSexId))
                {
                    throw new ArgumentException("演员性别参数错误，请刷新页面后重试");
                }
            }
            else if (resourceTypeId == 3)
            {
                if (!int.TryParse(model.EquipTypeId, out equipTypeId))
                {
                    throw new ArgumentException("设备类型参数错误，请刷新页面后重试");
                }
            }
            else
            {
                if (!int.TryParse(model.OtherTypeId, out otherTypeId))
                {
                    throw new ArgumentException("资源具体类型参数错误，请刷新页面后重试");
                }
            }

            if (quotePrice <= 0)
            {
                throw new ArgumentException("请填写该资源的报价金额");
            }

            if (string.IsNullOrEmpty(provinceId))
            {
                throw new ArgumentException("请选择资源地区");
            }
            if (string.IsNullOrEmpty(detailAddress))
            {
                throw new ArgumentException("请填写资源详细地址");
            }

            if (string.IsNullOrEmpty(contact))
            {
                throw new ArgumentException("请填写联系人姓名或称呼");
            }

            if (!RegExp.IsMobileNo(mobilePhone))
            {
                throw new ArgumentException("手机号码输入有误，请重新填写");
            }

            if (!string.IsNullOrEmpty(email) && !RegExp.IsEmail(email))
            {
                throw new ArgumentException("联系邮箱格式输入有误，请重新填写");
            }

            if (!string.IsNullOrEmpty(href) && !RegExp.IsUrl(href))
            {
                throw new ArgumentException("演出视频地址无效，请重新填写");
            }


            resource.UserId = CurrentUser.UserId;
            resource.ResourceType = resourceTypeId;

            resource.Title = title;
            resource.CanFriendlyLink = model.CanFriendlyLink;
            resource.ProvinceId = provinceId;
            resource.CityId = cityId;
            resource.AreaId = areaId;
            resource.CreateTime = currentTime;
            resource.SpaceSizeId = string.IsNullOrEmpty(model.SpaceSizeId) ? 1 : spaceSizeId;
            resource.ShortDesc = Witbird.SHTS.Common.Html.HtmlUtil.RemoveHTMLTags(model.Description);
            resource.Description = model.Description;
            resource.SpacePeopleId = string.IsNullOrEmpty(model.SpacePeopleId) ? 1 : spacePeopleId;
            resource.SpaceTreat = spaceTreat;
            resource.Contract = contact;
            resource.Email = email;
            resource.DetailAddress = detailAddress;
            resource.QQ = model.QQ;
            resource.Telephone = telephone;
            resource.Mobile = mobilePhone;
            resource.WeChat = model.WeChat;
            resource.Href = href;
            resource.ImageUrls = model.ImageUrls;
            resource.LastUpdatedTime = currentTime;
            resource.State = (int)ResourceState.Created;
            resource.ClickCount = 0;
            resource.ClickTime = currentTime;
            resource.UserName = CurrentUser.UserName;
            resource.QuotePrice = quotePrice;

            resource.ActorFromId = string.IsNullOrEmpty(model.ActorFromId) ? 1 : actorFromId;
            resource.ActorSex = string.IsNullOrEmpty(model.ActorSex) ? 1 : int.Parse(model.ActorSex);

            resource.SpaceTypeId = string.IsNullOrEmpty(model.SpaceTypeId) ? 0 : spaceTypeId;
            resource.ActorTypeId = string.IsNullOrEmpty(model.ActorTypeId) ? 0 : actorTypeId;
            resource.EquipTypeId = string.IsNullOrEmpty(model.EquipTypeId) ? 0 : equipTypeId;
            resource.OtherTypeId = string.IsNullOrEmpty(model.OtherTypeId) ? 0 : otherTypeId;

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
            else
            {
                if (resourceTypeId == 1)
                {
                    throw new ArgumentException("请选择至少一个场地特点");
                }
            }

            if (resource.ShortDesc != null && resource.ShortDesc.Length > 150)
            {
                resource.ShortDesc = resource.ShortDesc.Substring(0, 147) + "...";
            }

            return resource;
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
            //城市,分页,场地类型,场地特点,场地设施,场地大小,容纳人数,提供酒宴,开始时间,结束时间, 报价范围
            string[] filters = new[] { "ara", "city", "page", "sptp", "spft",
                "spfc", "spsz", "sppc", "sptt", "rsst", "rsed", "qpc" };

            return ProcessFiltersCore(filters);
        }
        private Dictionary<string, string> ProcessActorFilters()
        {
            //城市,分页,演员工作,开始时间,结束时间,来源,性别, 报价范围
            string[] filters = new[] { "ara", "city", "page", "actp", "rsst", "rsed", "acfm", "acsx", "qpc" };

            return ProcessFiltersCore(filters);
        }
        private Dictionary<string, string> ProcessEquipmentFilters()
        {
            //城市,分页,开始时间,结束时间,报价范围
            string[] filters = new[] { "ara", "city", "page", "eqtp", "rsst", "rsed", "qpc" };

            return ProcessFiltersCore(filters);
        }
        private Dictionary<string, string> ProcessOtherFilters()
        {
            string[] filters = new[] { "ara", "city", "page", "ottp", "rsst", "rsed", "qpc" };//城市,分页,开始时间,结束时间

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
