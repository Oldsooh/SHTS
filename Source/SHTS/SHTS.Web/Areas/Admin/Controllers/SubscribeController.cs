using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class SubscribeController : AdminBaseController
    {
        protected const string USERINFO = "userinfo";
        private DemandSubscriptionManager subscriptionManager = new DemandSubscriptionManager();

        [Permission(EnumRole.Normal)]
        public ActionResult Subscription(string id, int resourceTypeId = -1, int subResourceTypeId = -1,
            string province = "", string city = "", string area = "",
            string emailStatus = "-1", string keywords = "", string wechatUserNickName = "", string budgetCondition = "")
        {
            
            /*
             resourceTypeId=" + resourceTypeId +
                "&subResourceTypeId=" + subResourceTypeId +
                "&province=" + province +
                "&city=" + city +
                "&area=" + area +
                "&emailStatus=" + emailStatus +
                "&keywords=" + keywords +
                "wechatUserNickName" + wechatUserNickName;
             */
            SubscriptionModel model = new SubscriptionModel();

            //页码，总数重置
            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }
            int allCount = 0;

            model.FilterResourceType = resourceTypeId;
            model.FilterSubResourceType = subResourceTypeId;
            model.Province = province;
            model.City = city;
            model.Area = area;
            model.FilterEmailStatus = emailStatus;
            model.Keywords = keywords;
            model.FilterWechatUserNickName = wechatUserNickName;
            model.FilterBudget = budgetCondition;

            model.Subscriptions.AddRange(subscriptionManager.GetSubscriptions(15, page, out allCount, 
                resourceTypeId, subResourceTypeId, province, city, area, budgetCondition,
                emailStatus, keywords, wechatUserNickName));
            
            //分页
            if (model.Subscriptions != null && model.Subscriptions.Count > 0)
            {
                model.PageIndex = page;//当前页数
                model.PageSize = 15;//每页显示多少条
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

        [Permission(EnumRole.Normal)]
        public ActionResult PushHistory(string id, string demandIds="", string wechatUserNickName= "", 
            string wechatStatus="", string emailStatus="")
        {
            SubscriptionModel model = new SubscriptionModel();

            //页码，总数重置
            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }
            int allCount = 0;

            var demandIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(demandIds))
            {
                var idsArray = demandIds.Split(',');
                var demandId = -1;
                foreach (var item in idsArray)
                {
                    if (int.TryParse(item, out demandId))
                    {
                        demandIdList.Add(demandId);
                    }
                }
            }

            model.FilterDemandIdList.AddRange(demandIdList);
            model.FilterWechatStatus = wechatStatus;
            model.FilterWechatUserNickName = wechatUserNickName;
            model.FilterEmailStatus = emailStatus;

            model.PushHistories.AddRange(subscriptionManager.GetSubscriptionPushHistories(15, page, out allCount, 
                demandIdList, wechatUserNickName, wechatStatus, emailStatus));

            //分页
            if (model.PushHistories != null && model.PushHistories.Count > 0)
            {
                model.PageIndex = page;//当前页数
                model.PageSize = 15;//每页显示多少条
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
    }
}
