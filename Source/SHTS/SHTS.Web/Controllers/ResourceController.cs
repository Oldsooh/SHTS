using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL.New;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Controllers
{
    public class ResourceController : PCBaseController
    {
        ResourceManager resourceManager = new ResourceManager();
        ResourceService resourceService = new ResourceService();

        #region 创建资源
        [ActionName("create")]
        public ActionResult Create()
        {
            if (!IsIdentified)
            {
                return Redirect("/account/login");
            }
            else
            {
                ResourceViewModel spaceViewModel = new ResourceViewModel();
                return View("Create", spaceViewModel);
            }
        }

        [HttpPost]
        [ActionName("create")]
        [ValidateInput(false)]
        public ActionResult CreateResource(ResourceViewModel model)
        {
            if (!IsIdentified)
            {
                return Redirect("/account/login");
            }
            else
            {
                if (ModelState.IsValid)
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

                    return RedirectToAction("my");
                }
                return View(model);
            }



        }
        #endregion

        #region 编辑资源
        [ActionName("edit")]
        public ActionResult Edit(int id)
        {
            if (!IsIdentified)
            {
                return Redirect("/account/login");
            }
            else
            {
                var model = resourceManager.GetResourceById(id);
                if (model == null || model.UserId != UserInfo.UserId)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ResourceViewModel resource = new ResourceViewModel();

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
        public ActionResult Edit(ResourceViewModel model)
        {
            if (!IsIdentified)
            {
                return Redirect("/account/login");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    int id = int.Parse(model.ResId);

                    var resource = resourceManager.GetResourceById(id);
                    if (resource.UserId != UserInfo.UserId)
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        resource.ResourceType = int.Parse(model.ResType);

                        //resource.UserId = UserInfo.UserId;

                        resource.Title = model.Title;
                        resource.CanFriendlyLink = model.CanFriendlyLink;
                        resource.ProvinceId = model.ddlProvince;
                        resource.CityId = model.ddlCity;
                        resource.AreaId = model.ddlArea;
                        resource.CreateTime = DateTime.Now;
                        resource.SpaceSizeId = string.IsNullOrEmpty(model.SpaceSizeId) ? 0 : int.Parse(model.SpaceSizeId);
                        resource.ShortDesc = model.Description == null ? null : Witbird.SHTS.Common.Html.HtmlUtil.RemoveHTMLTags(model.Description);
                        resource.Description = model.Description;
                        resource.SpacePeopleId = string.IsNullOrEmpty(model.SpacePeopleId) ? 0 : int.Parse(model.SpacePeopleId);
                        resource.SpaceTreat = string.IsNullOrEmpty(model.SpaceTreat) ? 0 : int.Parse(model.SpaceTreat);
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

                        resource.ActorFromId = string.IsNullOrEmpty(model.ActorFromId) ? 0 : int.Parse(model.ActorFromId);
                        resource.ActorSex = string.IsNullOrEmpty(model.ActorSex) ? 0 : int.Parse(model.ActorSex);

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
                        else
                        {
                            resource.SpaceFacilityValue = 0;
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
                            resource.SpaceFeatureValue = 0;
                        }

                        if (resource.ShortDesc != null && resource.ShortDesc.Length > 150)
                        {
                            resource.ShortDesc = resource.ShortDesc.Substring(0, 147) + "...";
                        }

                        resource.SpaceTypeId = string.IsNullOrEmpty(model.SpaceTypeId) ? 0 : int.Parse(model.SpaceTypeId);
                        resource.ActorTypeId = string.IsNullOrEmpty(model.ActorTypeId) ? 0 : int.Parse(model.ActorTypeId);
                        resource.EquipTypeId = string.IsNullOrEmpty(model.EquipTypeId) ? 0 : int.Parse(model.EquipTypeId);
                        resource.OtherTypeId = string.IsNullOrEmpty(model.OtherTypeId) ? 0 : int.Parse(model.OtherTypeId);

                        resourceManager.EditResource(resource);

                        return RedirectToAction("my");
                    }
                }
                return View(model);
            }
        }
        #endregion

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

                int page = 0;
                if (!string.IsNullOrEmpty(Request.QueryString["page"]))
                {
                    int.TryParse(Request.QueryString["page"], out page);
                }
                page = page < 1 ? 1 : page;

                var result = resourceService.GetResourceByUser(UserInfo.UserId, page - 1, 15);

                return View(result);
            }
        }

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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (!IsIdentified)
                {
                    space.Telephone = "认证会员可见";
                    space.Mobile = "认证会员可见";
                    space.QQ = "认证会员可见";
                    space.WeChat = string.Empty;
                    space.Email = "认证会员可见";
                    space.Contract = "认证会员可见";
                    space.DetailAddress = "认证会员可见";
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
                ViewData["UserInfo"] = this.UserInfo;
                return View(space);
            }
        }


        #endregion

        public ActionResult Delete(string id)
        {
            if (!IsUserLogin)
            {
                return Redirect("/account/login");
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
                return Redirect("/account/login");
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
        //private DateTime minDate = new DateTime(1990, 1, 1);
        //private DateTime GetMinDateTimeFilterValue(Dictionary<string, string> filterDict, string filterName)
        //{
        //    DateTime value = minDate;
        //    if (filterDict != null &&
        //        !string.IsNullOrEmpty(filterName) &&
        //        filterDict.Keys.Contains(filterName) &&
        //        !string.IsNullOrEmpty(filterDict[filterName]) &&
        //        DateTime.TryParse(filterDict[filterName], out value))
        //    {
        //        if (value < minDate)
        //        {
        //            value = minDate;
        //        }
        //        if (value > maxDate)
        //        {
        //            value = maxDate;
        //        }
        //    }
        //    return value;
        //}
        //private DateTime maxDate = new DateTime(2050, 1, 1);
        //private DateTime GetMaxDateTimeFilterValue(Dictionary<string, string> filterDict, string filterName)
        //{
        //    DateTime value = maxDate;
        //    if (filterDict != null &&
        //        !string.IsNullOrEmpty(filterName)
        //        && filterDict.Keys.Contains(filterName) &&
        //        !string.IsNullOrEmpty(filterDict[filterName]) &&
        //        DateTime.TryParse(filterDict[filterName], out value))
        //    {
        //        if (value < minDate)
        //        {
        //            value = minDate;
        //        }
        //        if (value > maxDate)
        //        {
        //            value = maxDate;
        //        }
        //    }
        //    return value;
        //}

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
                //if (filter.Equals("rsst") || filter.Equals("rsed"))
                //{
                //    if (Regex.IsMatch(value, @"\d{4}-\d{2}-\d{2}"))
                //    {
                //        filterDict.Add(filter, value);
                //    }
                //    else
                //    {
                //        filterDict.Add(filter, string.Empty);
                //    }
                //}
                //else
                //{
                //    filterDict.Add(filter, value);
                //}
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

        /// <summary>
        /// 点评资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("comment")]
        public ActionResult Comment(int id)
        {
            if (!IsUserLogin)
            {
                return Redirect("/account/login");
            }
            else
            {

                CommentViewModel model = new CommentViewModel();

                if (id > 0)
                {
                    var resource = resourceManager.GetResourceById(id);
                    if (resource != null)
                    {
                        model.ResourceId = resource.Id.ToString();
                        model.Title = resource.Title;
                    }
                }
                else
                {
                    return Redirect("/");
                }

                return View(model);
            }
        }

        [HttpPost]
        [ActionName("comment")]
        [ValidateInput(false)]
        public ActionResult Comment(string rid, string content)
        {
            string message = ValidateCommentParameters(rid, content);

            if (string.IsNullOrWhiteSpace(message))
            {
                int resourceid = int.Parse(rid);
                var resource = resourceManager.GetResourceById(resourceid);

                if (resource != null)
                {
                    resourceManager.CommentOnResource(resource.Id, UserInfo.UserId, content);
                    message = "success";
                }
            }
            return Content(message);
        }

        private string ValidateCommentParameters(string rid, string content)
        {
            string errorMessage = string.Empty;

            if (!IsUserLogin)
            {
                errorMessage = "对不起，您还没有登录";
            }
            else if (!new UserService().ValidateUserResoureCommentPermission(UserInfo.UserId, int.Parse(rid)))
            {
                errorMessage = "对不起，只有对该资源进行中介申请的买家才能点评资源";
            }
            else if (string.IsNullOrWhiteSpace(content))
            {
                errorMessage = "点评内容不能为空";
            }

            return errorMessage;
        }
    }
}
