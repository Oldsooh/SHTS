using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Model;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class SubscribeController : WechatBaseController
    {
        //
        // GET: /Wechat/Subscribe/
        DemandSubscriptionService subscriptionService = new DemandSubscriptionService();

        public ActionResult Index()
        {
            DemandSubscription model = subscriptionService.GetSubscription(CurrentWeChatUser.Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(bool enable, string subscriedTypes, string subscribedAreas, string subscribedKeywords)
        {
            DemandSubscription subscription = subscriptionService.GetSubscription(CurrentWeChatUser.Id);

            try
            {
                subscriedTypes = subscriedTypes ?? string.Empty;
                subscribedAreas = subscribedAreas ?? string.Empty;
                subscribedKeywords = subscribedKeywords ?? string.Empty;

                // Clear old subscription details from cache.
                subscription.SubscriptionDetails.Clear();
                subscription.WeChatUserId = CurrentWeChatUser.Id;

                #region Handle subscribed demand categories
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
                #endregion

                #region Handle subscribed location areas
                var areas = subscribedAreas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (areas.Length > 0)
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

                #endregion

                #region Handle subscribed spcified keywords
                var keywords = subscribedKeywords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (keywords.Length > 0)
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

                subscription.IsSubscribed = enable;

                if (subscriptionService.UpdateSubscription(subscription))
                {
                    ViewData["UpdateSubscriptionResult"] = "更新订阅设置成功！";
                }
                else
                {
                    ViewData["UpdateSubscriptionResult"] = "更新订阅设置失败！";
                }
            }
            catch(Exception ex)
            {
                LogService.LogWexin("更新订阅设置失败", ex.ToString());
                ViewData["UpdateSubscriptionResult"] = "更新订阅设置失败！";
            }

            return View("Index", subscription);
        }
    }
}
