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
using WitBird.Common;

namespace Witbird.SHTS.Web.Controllers
{
    public class DemandController : PCBaseController
    {
        DemandManager demandManager = new DemandManager();
        DemandService demandService = new DemandService();
        CityService cityService = new CityService();

        public ActionResult Index(string page, string LastResourceType, string ResourceType, string ResourceTypeId, string area, string starttime, string endtime, string startbudget, string endbudget)
        {
            DemandModel model = new DemandModel();

            string city = string.Empty;
            if (!string.IsNullOrEmpty(CurrentCityId))
            {
                city = CurrentCityId;
            }
            int tempPage = 1;
            if (!string.IsNullOrEmpty(page))
            {
                Int32.TryParse(page, out tempPage);
            }

            // 每次更换需求列别需要重置需求类型选中的值
            if (string.IsNullOrEmpty(ResourceType))
            {
                ResourceTypeId = string.Empty;
            }
            if (!(LastResourceType ?? string.Empty).Equals(ResourceType, StringComparison.CurrentCultureIgnoreCase))
            {
                ResourceTypeId = string.Empty;
            }
            LastResourceType = ResourceType;

            //-------------------------------初始化页面参数（不含分页）-----------------------
            model.PageIndex = tempPage;
            model.LastResourceType = LastResourceType ?? string.Empty;
            model.ResourceType = ResourceType ?? string.Empty;
            model.ResourceTypeId = ResourceTypeId ?? string.Empty;
            model.City = city;
            model.Area = area ?? string.Empty;
            model.StartBudget = startbudget ?? string.Empty;
            model.EndBudget = endbudget ?? string.Empty;
            model.StartTime = starttime ?? string.Empty;
            model.EndTime = endtime ?? string.Empty;


            //-------------------------------初始化SQL查询参数-----------------------
            DemandParameters parameters = new DemandParameters();
            parameters.PageCount = 10;//每页显示10条 (与下面保持一致)
            parameters.PageIndex = tempPage;
            parameters.ResourceType = model.ResourceType;
            parameters.ResourceTypeId = model.ResourceTypeId;
            parameters.City = model.City;
            parameters.Area = model.Area;
            parameters.StartBudget = model.StartBudget;
            parameters.EndBudget = model.EndBudget;
            parameters.StartTime = model.StartTime;
            parameters.EndTime = model.EndTime;

            if (!string.IsNullOrEmpty(city))
            {
                model.Areas = cityService.GetAreasByCityId(city, true);
            }


            int allCount = 0;
            //需求分页列表,每页10条
            model.Demands = demandService.GetDemandsByParameters(parameters, out allCount);
            //分页
            if (model.Demands != null && model.Demands.Count > 0)
            {
                model.PageIndex = tempPage;//当前页数
                model.PageSize = 10;//每页显示多少条
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

        [HttpGet]
        public ActionResult Add()
        {
            if (!IsUserLogin)
            {
                return Redirect("/account/login");
            }
            DemandModel model = new DemandModel();
            model.Provinces = cityService.GetProvinces(true);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(int ResourceType, int? SpaceTypeId, int? ActorTypeId, int? EquipTypeId, int? OtherTypeId,
            string title, string contentStyle, //string description,
            string provinceId, string cityId, string areaId, string address, string phone, string qqweixin, string email,
            string startTime, string endTime, string timeLength, string peopleNumber, string budget)
        {
            string result = string.Empty;
            if (!IsUserLogin)
            {
                result = "未登录或登录超时！请在新窗口进行登录后返回继续操作！";
            }
            else
            {
                int resourceSubTypeId = -1;
                switch (ResourceType)
                {
                    case 1:
                        resourceSubTypeId = SpaceTypeId ?? -1;
                        break;
                    case 2:
                        resourceSubTypeId = ActorTypeId ?? -1;
                        break;
                    case 3:
                        resourceSubTypeId = EquipTypeId ?? -1;
                        break;
                    case 4:
                        resourceSubTypeId = OtherTypeId ?? -1;
                        break;
                    default:
                        break;
                }

                int demandBudget = 0;
                int.TryParse(budget, out demandBudget);
                int demandId = -1;

                if (demandManager.AddDemand(CurrentUser.UserId, ResourceType, resourceSubTypeId, title, contentStyle, provinceId,
                    cityId, areaId, address, phone, qqweixin, email, startTime, endTime, timeLength, peopleNumber,
                    demandBudget, (int)BuyDemandFee, string.Empty, out result, out demandId))
                {
                    Subscription.WorkingThread.Instance.SendDemandToSubscribers(demandId);
                }
            }

            return Content(result);
        }

        [HttpGet]
        public ActionResult MyDemands(string id)
        {
            if (Session[USERINFO] == null)
            {
                return Redirect("/account/login");
            }

            User user = null;
            user = Session[USERINFO] as User;

            DemandModel model = new DemandModel();
            

            //页码，总数重置
            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }
            int allCount = 0;
            model.Demands = demandService.GetDemandsByUserId(user.UserId, 20, page, out allCount);//每页显示20条
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

        public ActionResult Delete(string id)
        {
            if (Session[USERINFO] == null)
            {
                return Redirect("/account/login");
            }

            User user = null;
            user = Session[USERINFO] as User;

            if (!string.IsNullOrEmpty(id))
            {
                Demand demand = demandService.GetDemandById(Int32.Parse(id));
                if (demand != null && demand.UserId == user.UserId)
                {
                    demand.IsActive = false;
                    demandService.EditDemand(demand);
                }
            }

            return Redirect(Request.UrlReferrer.LocalPath);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (!IsUserLogin)
            {
                return Redirect("/account/login");
            }
            User user = null;

            DemandModel model = new DemandModel();

            model.Provinces = cityService.GetProvinces(true);//省

            if (!string.IsNullOrEmpty(id))
            {
                int tempId = 0;
                Int32.TryParse(id, out tempId);
                if (tempId != 0)
                {
                    model.Demand = demandService.GetDemandById(tempId);
                    if (model.Demand != null)
                    {
                        user = Session[USERINFO] as User;

                        if (user.UserId == model.Demand.UserId)
                        {
                            if (!string.IsNullOrEmpty(model.Demand.Province))
                            {
                                model.Cities = cityService.GetCitiesByProvinceId(model.Demand.Province, true);//市
                            }
                            if (!string.IsNullOrEmpty(model.Demand.City))
                            {
                                model.Areas = cityService.GetAreasByCityId(model.Demand.City, true);//区
                            }
                        }
                    }
                }
            }

            if (model.Demand == null)
            {
                model.Demand = new Demand();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int ResourceType, int? SpaceTypeId, int? ActorTypeId, int? EquipTypeId, int? OtherTypeId,
            string id, string title, //string description,
            string contentStyle,
            string provinceId, string cityId, string areaId, string address, string phone, string qqweixin, string email,
            string startTime, string endTime, string timeLength, string peopleNumber, string budget)
        {
            string result = "未更新";
            User user = null;
            if (!IsUserLogin)
            {
                result = "请登录后再编辑！";
            }
            else
            {
                if (!string.IsNullOrEmpty(id) &&
                    //!string.IsNullOrEmpty(ResourceType) &&
                    !string.IsNullOrEmpty(title) &&
                    //!string.IsNullOrEmpty(description) &&
                    !string.IsNullOrEmpty(contentStyle) &&
                    !string.IsNullOrEmpty(startTime) &&
                    !string.IsNullOrEmpty(endTime) &&
                    !string.IsNullOrEmpty(budget))
                {
                    user = Session[USERINFO] as User;
                    Demand demand = demandManager.GetDemandById(Int32.Parse(id));
                    if (demand.UserId == user.UserId)
                    {
                        demand.ResourceType = ResourceType;

                        switch (demand.ResourceType)
                        {
                            case 1:
                                demand.ResourceTypeId = SpaceTypeId;
                                break;
                            case 2:
                                demand.ResourceTypeId = ActorTypeId;
                                break;
                            case 3:
                                demand.ResourceTypeId = EquipTypeId;
                                break;
                            case 4:
                                demand.ResourceTypeId = OtherTypeId;
                                break;
                            default:
                                break;
                        }

                        demand.Title = title;
                        demand.Description = string.Empty;//description;
                        demand.ContentStyle = contentStyle;
                        demand.ContentText = Witbird.SHTS.Common.Html.HtmlUtil.RemoveHTMLTags(demand.ContentStyle);
                        demand.Province = string.IsNullOrEmpty(provinceId) ? string.Empty : provinceId;
                        demand.City = string.IsNullOrEmpty(cityId) ? string.Empty : cityId;
                        demand.Area = string.IsNullOrEmpty(areaId) ? string.Empty : areaId;
                        demand.Address = string.IsNullOrEmpty(address) ? string.Empty : address;
                        demand.Phone = phone;
                        demand.QQWeixin = string.IsNullOrEmpty(qqweixin) ? string.Empty : qqweixin;
                        demand.Email = string.IsNullOrEmpty(email) ? string.Empty : email;
                        demand.StartTime = DateTime.Parse(startTime);
                        demand.EndTime = DateTime.Parse(endTime);
                        demand.TimeLength = string.IsNullOrEmpty(timeLength) ? string.Empty : timeLength;
                        demand.PeopleNumber = string.IsNullOrEmpty(peopleNumber) ? string.Empty : peopleNumber;
                        int tempBudget = 0;
                        if (!string.IsNullOrEmpty(budget))
                        {
                            Int32.TryParse(budget, out tempBudget);
                        }
                        demand.Budget = tempBudget;
                        if (demand.EndTime < demand.StartTime)
                        {
                            result = "需求结束日期应不小于开始日期";
                        }
                        else if (demandService.EditDemand(demand))
                        {
                            result = "success";
                        }
                        else
                        {
                            result = "需求更新失败";
                        }

                    }
                    else
                    {
                        result = "只能对自己发布的需求进行更改";
                    }
                }
                else
                {
                    result = "必填项不能为空";
                }
            }
            return Content(result);
        }

        public ActionResult Show(string id)
        {
            DemandModel model = new DemandModel();

            int demandId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out demandId);
            }

            try
            {
                Demand demand = demandService.GetDemandById(demandId);
                if (demand != null)
                {
                    if (!demand.ContentStyle.Contains("<img"))
                    {
                        demand.ContentStyle = FilterHelper.Filter(FilterLevel.PhoneAndEmail, demand.ContentStyle, CommonService.ReplacementForContactInfo);
                        demand.Description = FilterHelper.Filter(FilterLevel.PhoneAndEmail, demand.Description, CommonService.ReplacementForContactInfo);
                        demand.Title = FilterHelper.Filter(FilterLevel.PhoneAndEmail, demand.Title, CommonService.ReplacementForContactInfo);
                    }

                    demand.ContentText = FilterHelper.Filter(FilterLevel.PhoneAndEmail, demand.ContentText, CommonService.ReplacementForContactInfo);

                    int userId = IsUserLogin ? CurrentUser.UserId : -1;
                    if (demand.UserId != userId && !IsVip)
                    {
                        demand.Phone = "VIP会员可见";
                        demand.QQWeixin = "VIP会员可见";
                        demand.Email = "VIP会员可见";
                        demand.Address = "详细地址VIP会员可见";
                    }
                    model.Demand = demand;
                }

                model.IsMember = IsUserLogin;
                model.IsVIP = IsVip;
            }
            catch (Exception e)
            {
                LogService.Log("需求详情", e.ToString());
            }

            if (model.Demand == null)
            {
                model.Demand = new Demand();
                model.Demand.StartTime = DateTime.Now;
                model.Demand.InsertTime = DateTime.Now;
                model.Demand.Title = "该需求不存在或已被删除";
            }

            return View(model);
        }

        public ActionResult UpdateDemandStatusAsComplete(int id)
        {
            if (!IsUserLogin)
            {
                return Redirect("/account/login");
            }

            if (id > -1)
            {
                demandService.UpdateDemandStatus(id, DemandStatus.Complete);
            }

            return Redirect(Request.UrlReferrer.LocalPath);
        }
    }
}
