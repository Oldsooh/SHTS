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

        static string AppId = ConfigurationManager.AppSettings["WeixinAppId"];
        static string AppSecret = ConfigurationManager.AppSettings["WeixinAppSecret"];

        const string RequestTimeExceedMessage = @"亲爱的需求订阅用户，由于微信服务号消息限制，48小时内如无主动交互我们将无法向您推送您订阅的最新需求信息。为了保证您的正常使用，你可以:
1. 点击菜单 需求信息 -> 获取订阅 以获取需求订阅内容并更新交互时间
2. 点击菜单 需求信息 -> 更新订阅 仅更新交互时间
3. 回复任意消息内容即可更新交互时间";

        const string NoResultFoundMessage = @"活动在线客服MM暂时还没发布符合您的订阅规则的相关需求哦，请稍后再试。";

        const string NotSubscriedMessage = @"由于您没有开启需求订阅，暂时无法获取需求内容。请点击菜单 需求信息 -> 需求订阅设置 来开启吧！开启后，您就可以免费收到最新发布的需求内容哦！";

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
                                    SendTextMessage(wechatUser, NotSubscriedMessage);
                                }
                                else
                                {
                                    // Selects demand and send to wechat user by subscription details.
                                    var lastPushTime = subscription.LastPushTimestamp ?? DateTime.Now.AddDays(-2);
                                    var demands = demandService.SelectDemandsForWeChatPush(subscription.SubscriptionDetails, lastPushTime);

                                    var isSendSuccessFul = false;
                                    if (demands.HasItem())
                                    {
                                        var articles = ConstructArticles(demands, lastPushTime);
                                        isSendSuccessFul = SendArticles(wechatUser, articles);
                                    }
                                    else
                                    {
                                        isSendSuccessFul = SendTextMessage(wechatUser, NoResultFoundMessage);
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
                                    // Message cannot be reached after 48 hours later
                                    if (!IsLastRequestTimeExceed48Hours(subscription))
                                    {
                                        // Gets demands by user's subscription details.
                                        var lastPushTime = subscription.LastPushTimestamp ?? DateTime.Now.AddDays(-2);
                                        var demands = demandService.SelectDemandsForWeChatPush(subscription.SubscriptionDetails, lastPushTime);

                                        if (demands.HasItem())
                                        {
                                            var articles = ConstructArticles(demands, lastPushTime);
                                            var isSendSuccessFul = SendArticles(wechatUser, articles);

                                            if (isSendSuccessFul)
                                            {
                                                // Update last push time
                                                subscriptionService.UpdateLastPushTimestamp(wechatUser.Id);
                                            }
                                        }

                                        // If last request time is closed to 48 hours, send remind message to update request time.
                                        if (IsLastRequestTimeExceed45Hours(subscription))
                                        {
                                            SendTextMessage(wechatUser, RequestTimeExceedMessage);
                                        }
                                    }
                                    else
                                    {
                                        // Send by email， to do
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.LogWexin("推送微信需求订阅发生异常", ex.ToString());
                    }

                    Thread.Sleep(PushInterval);
                }

            });
        }

        private bool IsLastRequestTimeExceed48Hours(DemandSubscription subscription)
        {
            if (!subscription.LastRequestTimestamp.HasValue)
            {
                return true;
            }

            return (DateTime.Now - subscription.LastRequestTimestamp.Value).TotalHours > 48;
        }

        private bool IsLastRequestTimeExceed45Hours(DemandSubscription subscription)
        {
            if (!subscription.LastRequestTimestamp.HasValue)
            {
                return true;
            }

            return (DateTime.Now - subscription.LastRequestTimestamp.Value).TotalHours >= 45;
        }

        private bool SendArticles(WeChatUser wechatUser, List<Article> articles)
        {
            bool isSuccessFul = false;
            AccessTokenContainer.Register(AppId, AppSecret);

            try
            {
                var wxResult = CustomApi.SendNews(AppId, wechatUser.OpenId, articles);
                if (wxResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
                {
                    isSuccessFul = true;
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("SendSubscribedDemands wechatUserId: " + wechatUser.Id, ex.ToString());
            }

            return isSuccessFul;
        }

        private bool SendTextMessage(WeChatUser wechatUser, string message)
        {
            bool isSuccessFul = false;
            AccessTokenContainer.Register(AppId, AppSecret);
            try
            {
                var wxResult = CustomApi.SendText(AppId, wechatUser.OpenId, message);
                if (wxResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
                {
                    isSuccessFul = true;
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("SendTextMessage, wechatUserId: " + wechatUser.Id, ex.ToString());
            }

            return isSuccessFul;
        }

        private List<Article> ConstructArticles(List<Demand> demands, DateTime lastPushTime)
        {
            List<Article> articles = new List<Article>();

            if (demands.Count > 7)
            {
                // Construct view more item
                var viewMore = new Article()
                {
                    Description = "查看更多需求信息",
                    PicUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/content/images/subscribedDemandBackground.jpg",
                    Title = "您有新的" + demands.Count + "条需求推荐信息，以下共显示7条，点击查看所有推荐",
                    Url = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/subscribe/viewall/?ticks=" + lastPushTime.Ticks.ToString()
                };

                articles.Add(viewMore);
            }
            else
            {
                // Construct view more item
                var viewMore = new Article()
                {
                    Description = "",
                    PicUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/content/images/subscribedDemandBackground.jpg",
                    Title = "您有新的" + demands.Count + "条需求推荐信息，立即查看",
                    Url = string.Empty
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