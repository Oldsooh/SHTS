using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Common;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using System.Threading;
using System.Threading.Tasks;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using System.Configuration;
using Witbird.SHTS.Web.Areas.Wechat.Common;


namespace Witbird.SHTS.Web.Areas.Wechat.Subscription
{
    /// <summary>
    /// 微信推送管理线程
    /// </summary>
    public class WorkingThread
    {
        TimeSpan PushInterval = new TimeSpan(0, 30, 0);
        bool isRunning = true;
        static WorkingThread instance = null;
        DemandSubscriptionService subscriptionService = null;
        UserService userService = null;
        DemandService demandService = null;

        public WorkingThread()
        {
            subscriptionService = new DemandSubscriptionService();
            userService = new UserService();
            demandService = new DemandService();
        }

        public static WorkingThread Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorkingThread();
                }

                return instance;
            }
        }

        public void SendSubscribedDemandManually(string openId)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(openId))
                    {
                        var wechatUser = userService.GetWeChatUser(openId);
                        if (wechatUser.IsNotNull())
                        {
                            var subscription = subscriptionService.GetSubscription(wechatUser.Id);
                            if (subscription.IsNotNull())
                            {
                                // Current user does not subscribe demands.
                                if (!subscription.IsSubscribed)
                                {
                                    WeChatClient.Sender.SendText(wechatUser.OpenId, WeChatClient.Constant.NotEnableSubscription);
                                }
                                else
                                {
                                    // Selects demand and send to wechat user by subscription details.
                                    var lastPushTime = subscription.LastPushTimestamp ?? DateTime.Now.AddDays(-2);
                                    var demands = demandService.SelectDemandsForWeChatPush(subscription.SubscriptionDetails, lastPushTime);

                                    var isSendSuccessFul = false;
                                    if (demands.HasItem())
                                    {
                                        var articles = ConstructArticles(demands);
                                        isSendSuccessFul = WeChatClient.Sender.SendArticles(wechatUser.OpenId, articles);
                                    }
                                    else
                                    {
                                        isSendSuccessFul = WeChatClient.Sender.SendText(wechatUser.OpenId, WeChatClient.Constant.NoSubcribedDemansFound);
                                    }

                                    if (isSendSuccessFul)
                                    {
                                        // Update last push time
                                        subscriptionService.UpdateLastPushTimestamp(wechatUser.Id);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogWexin("手动获取需求推送发生异常", ex.ToString());
                }
            });
        }

        /// <summary>
        /// Starts to dequeue demands and push to wechat user client according to user's subscription rules.
        /// </summary>
        public void Run()
        {
            Task.Factory.StartNew(() =>
            {
                while (isRunning)
                {
                    try
                    {
                        LogService.LogWexin("微信推送开始, 当前时间：" + DateTime.Now.ToString(), string.Empty);
                        
                        var totalSubscribedUserCount = 0;
                        var totalPushSuccessfulCount = 0;

                        // Get all subscribed details.
                        var subscriptions = subscriptionService.GetSubscriptionsOnlySubscribed();
                        // Get all subscribed wechat users.
                        var subscribedWeChatUsers = userService.GetWeChatUserOnlySubscribed();

                        if (subscriptions.HasItem() && subscribedWeChatUsers.HasItem())
                        {
                            foreach (var subscription in subscriptions)
                            {
                                // Only subscried user can recieve articles.
                                var wechatUser = subscribedWeChatUsers.FirstOrDefault(x => x.Id == subscription.WeChatUserId);
                                if (wechatUser.IsNotNull())
                                {
                                    totalSubscribedUserCount++;

                                    // Message cannot be reached after 48 hours later
                                    if (!IsLastRequestTimeExceed48Hours(wechatUser))
                                    {
                                        // Gets demands by user's subscription details.
                                        var lastPushTime = subscription.LastPushTimestamp ?? DateTime.Now.AddDays(-2);
                                        var demands = demandService.SelectDemandsForWeChatPush(subscription.SubscriptionDetails, lastPushTime);

                                        if (demands.HasItem())
                                        {
                                            var articles = ConstructArticles(demands);
                                            var isSendSuccessFul = WeChatClient.Sender.SendArticles(wechatUser.OpenId, articles);

                                            if (isSendSuccessFul)
                                            {
                                                // Update last push time
                                                subscriptionService.UpdateLastPushTimestamp(wechatUser.Id);
                                                totalPushSuccessfulCount++;
                                            }
                                        }

                                        // If last request time is closed to 48 hours, send remind message to update request time.
                                        if (IsLastRequestTimeExceed47Hours(wechatUser))
                                        {
                                            WeChatClient.Sender.SendText(wechatUser.OpenId, WeChatClient.Constant.RequestTimeExceedMessage);
                                        }
                                    }
                                    else
                                    {
                                        // Send by email， to do
                                    }
                                }
                            }
                        }

                        LogService.LogWexin("微信推送结束, 当前时间：" + DateTime.Now.ToString(), 
                            "订阅用户共" + totalSubscribedUserCount + "人，成功推送给" + totalPushSuccessfulCount + "人");
                    }
                    catch (Exception ex)
                    {
                        LogService.LogWexin("推送微信需求订阅发生异常", ex.ToString());
                    }

                    Thread.Sleep(PushInterval);
                }

            });
        }

        private bool IsLastRequestTimeExceed48Hours(WeChatUser wechatUser)
        {
            if (!wechatUser.LastRequestTimestamp.HasValue)
            {
                return true;
            }

            return (DateTime.Now - wechatUser.LastRequestTimestamp.Value).TotalHours > 48;
        }

        private bool IsLastRequestTimeExceed47Hours(WeChatUser wechatUser)
        {
            if (!wechatUser.LastRequestTimestamp.HasValue)
            {
                return true;
            }

            return (DateTime.Now - wechatUser.LastRequestTimestamp.Value).TotalHours >= 47;
        }
        
        private List<Article> ConstructArticles(List<Demand> demands)
        {
            List<Article> articles = new List<Article>();

            if (demands.Count > 7)
            {
                // Construct view more item
                var viewMore = new Article()
                {
                    Description = "查看更多需求信息",
                    PicUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/content/images/subscribed.jpg",
                    Title = "您有新的" + demands.Count + "条需求推荐信息，以下共显示7条。点击查看今日所有推荐",
                    Url = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/subscribe/viewall/?ticks=" + DateTime.Today.Ticks.ToString()
                };

                articles.Add(viewMore);
            }
            else
            {
                // Construct view more item
                var viewMore = new Article()
                {
                    Description = "",
                    PicUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/content/images/subscribed.jpg",
                    Title = "您有新的" + demands.Count + "条需求推荐信息。点击查看今日所有推荐",
                    Url = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/subscribe/viewall/?ticks=" + DateTime.Today.Ticks.ToString()
                };

                articles.Add(viewMore);
            }

            for (int i = 0; i < demands.Count; i++)
            {
                // The wechat articles limited to 8 item in total.
                if (i >= 7)
                {
                    break;
                }

                var demand = demands[i];
                
                var article = new Article()
                {
                    Description = demand.Description,
                    PicUrl = string.Empty,
                    Title = demand.Title,
                    Url = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/demand/show/" + demand.Id + "?showwxpaytitle=1"
                };

                articles.Add(article);
            }

            return articles;
        }
    }
}