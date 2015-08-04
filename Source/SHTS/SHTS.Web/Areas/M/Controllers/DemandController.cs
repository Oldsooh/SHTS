using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.M.Models;

namespace Witbird.SHTS.Web.Areas.M.Controllers
{
    public class DemandController : MobileBaseController
    {
        protected const string USERINFO = "userinfo";

        DemandManager demandManager = new DemandManager();
        DemandService demandService = new DemandService();
        CityService cityService = new CityService();

        public ActionResult Index(string id)
        {
            DemandModel model = new DemandModel();
            model.DemandCategories = demandManager.GetDemandCategories();

            string city = string.Empty;
            if (Session["CityId"] != null)
            {
                city = Session["CityId"].ToString();
            }

            //页码，总数重置
            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }
            int allCount = 0;
            if (string.IsNullOrEmpty(city))
            {
                model.Demands = demandService.GetDemands(10, page, out allCount);//每页显示10条
            }
            else
            {
                model.Demands = demandService.GetDemandsByCity(10, page, city, out allCount);//每页显示10条
            }
            //分页
            if (model.Demands != null && model.Demands.Count > 0)
            {
                model.PageIndex = page;//当前页数
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
                if (!IsVip)
                {
                    demand.Phone = "VIP会员可见";
                    demand.QQWeixin = "VIP会员可见";
                    demand.Email = "VIP会员可见";
                    demand.Address = "VIP会员可见";
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

            model.IsMember = IsUserLogin;
            model.IsVIP = IsVip;

            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!IsUserLogin)
            {
                return Redirect("/m/account/login");
            }
            DemandModel model = new DemandModel();
            model.DemandCategories = demandManager.GetDemandCategories();
            model.Provinces = cityService.GetProvinces(true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(Demand demand)
        {
            string result = "发布失败";
            if (!IsUserLogin)
            {
                result = "长时间为操作，请重新登录";
            }
            else if (demand != null)
            {
                if (!string.IsNullOrEmpty(demand.Title) &&
                    !string.IsNullOrEmpty(demand.ContentText) &&
                    demand.CategoryId != 0)
                {
                    User user = Session[USERINFO] as User;
                    demand.UserId = user.UserId;
                    demand.IsActive = true;
                    demand.InsertTime = DateTime.Now;
                    demand.StartTime = demand.InsertTime;
                    demand.EndTime = demand.StartTime;
                    demand.ContentStyle = demand.ContentText;
                    if (demand.ContentText.Length > 291)
                    {
                        demand.Description = demand.ContentText.Substring(0, 290);
                    }
                    else
                    {
                        demand.Description = demand.ContentText;
                    }
                    if (demand.Budget == null)
                    {
                        demand.Budget = 0;
                    }
                    if (demandManager.AddDemand(demand))
                    {
                        result = "success";
                    }
                }
                else
                {
                    result = "必须项不能为空";
                }
            }
            return Content(result);
        }
    }
}
