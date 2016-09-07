using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Model;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Web.Areas.Wechat.Models;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class SubscribeController : WechatBaseController
    {
        //
        // GET: /Wechat/Subscribe/
        DemandSubscriptionService subscriptionService = new DemandSubscriptionService();
        DemandService demandService = new DemandService();

        public ActionResult Index()
        {
            DemandSubscription model = subscriptionService.GetSubscription(CurrentWeChatUser.Id);
            return View(model);
        }

        public ActionResult ViewAll(long ticks)
        {
            DemandModel model = new DemandModel();

            try
            {
                var subscription = subscriptionService.GetSubscription(CurrentWeChatUser.Id);
                if (subscription.IsNotNull())
                {
                    //var lastPushTime = new DateTime(ticks);
                    var lastPushTime = DateTime.Today;// 今日所有推荐

                    // Gets demands by user's subscription details.
                    var demands = demandService.SelectDemandsForWeChatPush(subscription.SubscriptionDetails, lastPushTime);

                    model.Demands = demands;
                }
            }

            catch (Exception ex)
            {
                LogService.LogWexin("查看所有推荐出错", ex.ToString());
            }

            if (!model.Demands.IsNotNull())
            {
                model.Demands = new List<Demand>();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(bool enable, string subscriedTypes, string subscribedAreas, string subscribedKeywords, bool emailEnabled, string emailAddress)
        {
            DemandSubscription subscription = subscriptionService.GetSubscription(CurrentWeChatUser.Id);
            var errorMessage = string.Empty;
            var isValid = true;

            try
            {
                subscriedTypes = subscriedTypes ?? string.Empty;
                subscribedAreas = subscribedAreas ?? string.Empty;
                subscribedKeywords = subscribedKeywords ?? string.Empty;

                // Clear old subscription details from cache.
                subscription.SubscriptionDetails.Clear();
                subscription.WeChatUserId = CurrentWeChatUser.Id;

                #region Handle subscribed types
                var types = subscriedTypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (types.Length > 0)
                {
                    foreach (var value in types)
                    {
                        var detail = new DemandSubscriptionDetail()
                        {
                            SubscriptionId = subscription.SubscriptionId,
                            SubscriptionType = DemandSubscriptionType.Category.ToString(),
                            InsertedTimestamp = DateTime.Now,
                            SubscriptionValue = value
                        };

                        subscription.SubscriptionDetails.Add(detail);

                    }
                }
                else
                {
                    if (enable)
                    {
                        errorMessage = "请选择需要订阅的需求类别与类型！";
                        isValid = false;
                    }
                }
                #endregion

                #region Handle subscribed location areas
                var areas = subscribedAreas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (isValid && areas.Length > 0)
                {
                    foreach (var value in areas)
                    {
                        var detail = new DemandSubscriptionDetail()
                        {
                            SubscriptionId = subscription.SubscriptionId,
                            SubscriptionType = DemandSubscriptionType.Area.ToString(),
                            InsertedTimestamp = DateTime.Now,
                            SubscriptionValue = value
                        };

                        subscription.SubscriptionDetails.Add(detail);
                    }
                }
                else
                {
                    if (enable)
                    {
                        errorMessage = "请选择需求类型所属的区域位置！";
                        isValid = false;
                    }
                }

                #endregion

                #region Handle subscribed spcified keywords
                var keywords = subscribedKeywords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (isValid && keywords.Length > 0)
                {
                    foreach (var value in keywords)
                    {
                        var detail = new DemandSubscriptionDetail()
                        {
                            SubscriptionId = subscription.SubscriptionId,
                            SubscriptionType = DemandSubscriptionType.Keywords.ToString(),
                            InsertedTimestamp = DateTime.Now,
                            SubscriptionValue = value
                        };

                        subscription.SubscriptionDetails.Add(detail);

                    }
                }

                #endregion

                if (isValid)
                {
                    subscription.IsSubscribed = enable;
                    subscription.IsEnableEmailSubscription = emailEnabled;

                    subscription.EmailAddress = emailAddress;
                    isValid = subscriptionService.UpdateSubscription(subscription);
                }

                if (isValid)
                {
                    errorMessage = "success";
                }
                else
                {
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage = "更新订阅设置失败！";
                    }

                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("更新订阅设置失败", ex.ToString());
                errorMessage = "更新订阅设置失败！";
            }
            
            ViewData["UpdateSubscriptionResult"] = errorMessage;

            return View("Index", subscription);
        }
    }
}
