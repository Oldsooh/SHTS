using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Mobile.Models;
using Witbird.SHTS.Web.Public;
using WitBird.Common;

namespace Witbird.SHTS.Web.Areas.Mobile.Controllers
{
    public class DemandController : MobileBaseController
    {
        DemandManager demandManager = new DemandManager();
        DemandService demandService = new DemandService();
        OrderService orderService = new OrderService();
        CityService cityService = new CityService();
        DemandQuoteService quoteService = new DemandQuoteService();

        public ActionResult Index(string page, string LastResourceType, string ResourceType, string ResourceTypeId, string area, string starttime, string endtime, string startbudget, string endbudget)
        {
            DemandModel model = new DemandModel();

            string city = string.Empty;
            if (Session["CityId"] != null)
            {
                city = Session["CityId"].ToString();
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

        public ActionResult Show(string id)
        {
            DemandModel model = new DemandModel();

            int tempId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out tempId);
            }

            if (tempId != 0)
            {
                var demand = demandService.GetDemandById(tempId);

                if (demand != null)
                {
                    demand.ContentText = FilterHelper.Filter(FilterLevel.PhoneAndEmail, demand.ContentText, CommonService.ReplacementForContactInfo);
                    demand.ContentStyle = FilterHelper.Filter(FilterLevel.PhoneAndEmail, demand.ContentStyle, CommonService.ReplacementForContactInfo);
                    demand.Description = FilterHelper.Filter(FilterLevel.PhoneAndEmail, demand.Description, CommonService.ReplacementForContactInfo);
                    demand.Title = FilterHelper.Filter(FilterLevel.PhoneAndEmail, demand.Title, CommonService.ReplacementForContactInfo);

                    if (demand.UserId != (CurrentUser ?.UserId) && !IsVip)
                    {
                        demand.Phone = "VIP会员可见";
                        demand.QQWeixin = "VIP会员可见";
                        demand.Email = "VIP会员可见";
                        demand.Address = "VIP会员可见";
                    }
                }
                
                model.Demand = demand;
            }

            if (model.Demand == null)
            {
                model.Demand = new Demand();
                model.Demand.Title = "该需求不存在或已被删除";
                model.Demand.StartTime = DateTime.Now;
                model.Demand.EndTime = DateTime.Now;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!IsUserLogin)
            {
                return Redirect("/mobile/account/login");
            }
            DemandModel model = new DemandModel();
            model.Provinces = cityService.GetProvinces(true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(int ResourceType, int? SpaceTypeId, int? ActorTypeId, int? EquipTypeId, int? OtherTypeId,
            string title, string contentText,
            string province, string city, string area, string address, string phone, string qqweixin, string email,
            string startTime, string endTime, string peopleNumber, string budget)
        {
            string result = "发布失败";
            if (!IsUserLogin)
            {
                result = "您还未登录或登录已过期，请重新登录后操作！";
            }
            else
            {
                if (ResourceType < 1)
                {
                    result = "请选择需求类别";
                }
                else if (!string.IsNullOrEmpty(title) &&
                    !string.IsNullOrEmpty(contentText) &&
                    !string.IsNullOrEmpty(startTime) &&
                    !string.IsNullOrEmpty(endTime) &&
                    !string.IsNullOrEmpty(budget) &&
                    !string.IsNullOrEmpty(phone))
                {
                    User user = CurrentUser;
                    Demand demand = new Demand();
                    demand.UserId = user.UserId;
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
                    demand.ContentText = contentText;
                    demand.ContentStyle = demand.ContentText;

                    if (demand.ContentText.Length > 291)
                    {
                        demand.Description = demand.ContentText.Substring(0, 290);
                    }
                    else
                    {
                        demand.Description = demand.ContentText;
                    }

                    demand.Province = string.IsNullOrEmpty(province) ? string.Empty : province;
                    demand.City = string.IsNullOrEmpty(city) ? string.Empty : city;
                    demand.Area = string.IsNullOrEmpty(area) ? string.Empty : area;
                    demand.Address = string.IsNullOrEmpty(address) ? string.Empty : address;
                    demand.Phone = phone;
                    demand.QQWeixin = string.IsNullOrEmpty(qqweixin) ? string.Empty : qqweixin;
                    demand.Email = string.IsNullOrEmpty(email) ? string.Empty : email;
                    demand.StartTime = DateTime.Parse(startTime);
                    demand.EndTime = DateTime.Parse(endTime);
                    demand.TimeLength = string.Empty;
                    demand.PeopleNumber = string.IsNullOrEmpty(peopleNumber) ? string.Empty : peopleNumber;
                    int tempBudget = 0;
                    if (!string.IsNullOrEmpty(budget))
                    {
                        Int32.TryParse(budget, out tempBudget);
                    }
                    demand.Budget = tempBudget;
                    demand.IsActive = true;
                    demand.ViewCount = 0;
                    demand.InsertTime = DateTime.Now;
                    demand.Status = (int)DemandStatus.InProgress;
                    demand.WeixinBuyFee = (int)BuyDemandFee;

                    if (demandManager.AddDemand(demand))
                    {
                        result = "success";
                        Subscription.WorkingThread.Instance.SendDemandByEmail(demand.Id);
                    }
                }
                else
                {
                    result = "请将需求信息补充完整并检查其正确性";
                }
            }
            return Content(result);
        }

        /// <summary>
        /// 我发布的需求记录
        /// </summary>
        /// <returns></returns>
        public ActionResult MyDemands(string page)
        {
            if (!IsUserLogin)
            {
                return Redirect("/mobile/account/login");
            }

            DemandModel model = new DemandModel();
            model.ActionName = "MyDemands";

            //页码，总数重置
            int pageIndex = 1;
            if (!string.IsNullOrEmpty(page))
            {
                Int32.TryParse(page, out pageIndex);
            }
            int allCount = 0;
            model.Demands = demandService.GetDemandsByUserId(CurrentUser.UserId, 20, pageIndex, out allCount);//每页显示20条
            //分页
            if (model.Demands != null && model.Demands.Count > 0)
            {
                model.PageIndex = pageIndex;//当前页数
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

        public ActionResult UpdateDemandStatusAsComplete(int id)
        {
            if (id > -1)
            {
                demandService.UpdateDemandStatus(id, DemandStatus.Complete);
            }

            return Redirect(Request.UrlReferrer.OriginalString);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!IsUserLogin)
            {
                return Redirect("/mobile/account/login");
            }
            DemandModel model = new DemandModel();

            model.Provinces = cityService.GetProvinces(true);//省

            if (id > 0)
            {
                model.Demand = demandService.GetDemandById(id);
                if (model.Demand != null)
                {
                    if (CurrentUser.UserId == model.Demand.UserId)
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
            if (model.Demand == null)
            {
                model.Demand = new Demand();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, int ResourceType, int? SpaceTypeId, int? ActorTypeId, int? EquipTypeId, int? OtherTypeId,
            string title, string contentText,
            string province, string city, string area, string address, string phone, string qqweixin, string email,
            string startTime, string endTime, string peopleNumber, string budget)
        {
            string result = "更新需求失败";
            if (!IsUserLogin)
            {
                result = "长时间为操作，请重新登录";
            }
            else
            {
                if (ResourceType < 1)
                {
                    result = "请选择需求类别";
                }
                else if (id > 0 &&
                    !string.IsNullOrEmpty(title) &&
                    !string.IsNullOrEmpty(contentText) &&
                    !string.IsNullOrEmpty(startTime) &&
                    !string.IsNullOrEmpty(endTime) &&
                    !string.IsNullOrEmpty(budget) &&
                    !string.IsNullOrEmpty(phone))
                {
                    Demand demand = demandManager.GetDemandById(id);

                    if (demand != null)
                    {
                        if (demand.UserId != CurrentUser.UserId)
                        {
                            result = "只能对自己发布的需求进行编辑";
                        }
                        else if (demand.IsCompleted)
                        {
                            result = "该需求已完成，不能进行编辑";
                        }
                        else
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
                            demand.ContentText = contentText;
                            demand.ContentStyle = demand.ContentText;

                            if (demand.ContentText.Length > 291)
                            {
                                demand.Description = demand.ContentText.Substring(0, 290);
                            }
                            else
                            {
                                demand.Description = demand.ContentText;
                            }

                            demand.Province = string.IsNullOrEmpty(province) ? string.Empty : province;
                            demand.City = string.IsNullOrEmpty(city) ? string.Empty : city;
                            demand.Area = string.IsNullOrEmpty(area) ? string.Empty : area;
                            demand.Address = string.IsNullOrEmpty(address) ? string.Empty : address;
                            demand.Phone = phone;
                            demand.QQWeixin = string.IsNullOrEmpty(qqweixin) ? string.Empty : qqweixin;
                            demand.Email = string.IsNullOrEmpty(email) ? string.Empty : email;
                            demand.StartTime = DateTime.Parse(startTime);
                            demand.EndTime = DateTime.Parse(endTime);
                            demand.TimeLength = string.Empty;
                            demand.PeopleNumber = string.IsNullOrEmpty(peopleNumber) ? string.Empty : peopleNumber;
                            int tempBudget = 0;
                            if (!string.IsNullOrEmpty(budget))
                            {
                                Int32.TryParse(budget, out tempBudget);
                            }
                            demand.Budget = tempBudget;
                            demand.IsActive = true;
                            demand.ViewCount = 0;
                            demand.InsertTime = DateTime.Now;
                            demand.Status = (int)DemandStatus.InProgress;

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
                    }
                    else
                    {
                        result = "需求不存在或已被删除";
                    }
                }
                else
                {
                    result = "请将需求信息补充完整并检查其正确性";
                }
            }
            return Content(result);
        }
    }
}
