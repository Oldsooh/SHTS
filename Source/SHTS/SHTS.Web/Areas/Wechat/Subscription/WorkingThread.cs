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
        TimeSpan PushInterval = new TimeSpan(1, 0, 0);
        // For test use only.
        //TimeSpan PushInterval = new TimeSpan(0, 1, 0);
        bool isRunning = true;
        static WorkingThread instance = null;
        DemandSubscriptionService subscriptionService = null;
        UserService userService = null;
        DemandService demandService = null;

        static string AppId = ConfigurationManager.AppSettings["WeixinAppId"];
        static string AppSecret = ConfigurationManager.AppSettings["WeixinAppSecret"];

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

        public void Run()
        {
            Task.Factory.StartNew(() =>
            {
                while (isRunning)
                {
                    try
                    {
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
                                    // Message cannot be reached after 48 hours later.
                                    if (!IsLastRequestTimeExceed48Hours(subscription))
                                    {
                                        var lastPushTime = subscription.LastPushTimestamp ?? DateTime.Now.AddDays(-2);

                                        // Send demands by user's subscription details.
                                        var demands = demandService.SelectDemandsForWeChatPush(subscription.SubscriptionDetails, lastPushTime);

                                        if (demands.HasItem())
                                        {
                                            var isSendSuccessFul = SendSubscribedDemands(wechatUser, demands);

                                            if (isSendSuccessFul)
                                            {
                                                // Update last push time
                                                subscriptionService.UpdateLastPushTimestamp(wechatUser.Id);
                                            }
                                        }

                                        // If last request time is closed to 48 hours, send remind message to update request time.
                                        if (IsLastRequestTimeExceed45Hours(subscription))
                                        {
                                            SendRemindMessage(wechatUser);
                                        }
                                    }
                                    else
                                    {
                                        // Nothing to do.
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

        private bool SendSubscribedDemands(WeChatUser wechatUser, List<Demand> demands)
        {
            bool isSuccessFul = false;
            AccessTokenContainer.Register(AppId, AppSecret);

            try
            {
                var articles = ConstructArticles(demands);

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

        private void SendRemindMessage(WeChatUser wechatUser)
        {
            AccessTokenContainer.Register(AppId, AppSecret);
            try
            {
                var message = @"亲爱的活动在线客户，由于微信服务号消息限制，48小时内如无主动交互我们将无法向您推送您订阅的最新需求信息。为了保证您的正常使用，请更新交互时间:
1. 点击菜单 会员中心 -> 更新交互时间
2. 直接回复任意消息内容即可更新";
                var wxResult = CustomApi.SendText(AppId, wechatUser.OpenId, message);
            }
            catch (Exception ex)
            {
                LogService.LogWexin("SendRemindMessage, wechatUserId: " + wechatUser.Id, ex.ToString());
            }
        }

        private List<Article> ConstructArticles(List<Demand> demands)
        {
            List<Article> articles = new List<Article>();

            for (int i = 0; i < demands.Count; i++)
            {
                // The wechat articles limited to 7 item in total.
                if (i >= 7)
                {
                    break;
                }

                var demand = demands[i];
                // First demand will add a background image, others not.
                if (i == 0)
                {
                    var article = new Article()
                    {
                        Description = demand.Description,
                        PicUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/content/images/subscribedDemandBackground.jpg",
                        Title = demand.Title,
                        Url = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/demand/show/" + demand.Id + "?showwxpaytitle=1"
                    };

                    articles.Add(article);
                }
                else
                {
                    var article = new Article()
                    {
                        Description = demand.Description,
                        PicUrl = string.Empty,
                        Title = demand.Title,
                        Url = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/demand/show/" + demand.Id + "?showwxpaytitle=1"
                    };

                    articles.Add(article);
                }
            }

            // Construct view more item
            var viewMore = new Article()
            {
                Description = "查看更多需求信息",
                PicUrl = string.Empty,
                Title = "查看更多需求信息",
                Url = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/demand/index?showwxpaytitle=1"
            };

            articles.Add(viewMore);

            return articles;
        }
    }
}