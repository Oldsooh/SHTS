using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Common;
using Witbird.SHTS.Web.Public;

namespace Witbird.SHTS.Web.Subscription
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
        private DemandSubscriptionManager subscriptionManager;
        UserService userService = null;
        DemandService demandService = null;
        private static object locker = new object();

        public WorkingThread()
        {
            subscriptionService = new DemandSubscriptionService();
            subscriptionManager = new DemandSubscriptionManager();
            userService = new UserService();
            demandService = new DemandService();
        }

        public static WorkingThread Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new WorkingThread();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 用户输入关键字主动获取
        /// </summary>
        /// <param name="openId"></param>
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
        /// 定时任务推送，已未使用
        [Obsolete]
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

        /// <summary>
        /// 需求发布时推送
        /// </summary>
        /// <param name="demandId"></param>
        public void SendDemandToSubscribers(int demandId)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var demand = demandService.GetDemandById(demandId);
                    if (demand == null)
                    {
                        return;
                    }

                    // Get all subscribed details.
                    var subscriptions = subscriptionService.GetSubscriptionsOnlySubscribed();
                    // Get email subscription format.
                    PublicConfigService configService = new PublicConfigService();
                    // {0}:取消订阅链接， {1}：需求标题，{2}：需求类型，{3}：需求类别，{4}：需求预算，{5}：需求开始时间和结束时间，{6}：区域位置，{7}：需求详情，{8}：查看需求链接
                    var emailSubscriptionContentFormat = configService.GetConfigValue("EmailSubscriptionContentFormat").ConfigValue;

                    // 等待被推送的用户
                    var matchedSubscriptions = new List<DemandSubscription>();

                    if (subscriptions.HasItem())
                    {
                        foreach (var subscription in subscriptions)
                        {
                            bool isDemandMatchWithSubscription = false;

                            // if demand was posted by himself, then execlue.
                            if (subscription.UserId.HasValue && demand.UserId != subscription.UserId.Value)
                            {
                                isDemandMatchWithSubscription = IsDemandMatchWithSubscription(subscription, demand);
                            }

                            if (isDemandMatchWithSubscription)
                            {
                                matchedSubscriptions.Add(subscription);
                            }
                        }

                        #region 邮箱推送
                        var matchedEmails = matchedSubscriptions
                            .Where(item => item.IsEnableEmailSubscription.HasValue &&
                                           item.IsEnableEmailSubscription.Value &&
                                           !string.IsNullOrEmpty(item.EmailAddress))
                            .Select(item => item.EmailAddress).ToList();
                        var matchOpenIds = matchedSubscriptions.Select(item => item.OpenId).ToList();
                        var isEmailPushSuccess = false;

                        if (matchedEmails.Any())
                        {
                            try
                            {
                                var displayName = "活动在线网";
                                var mailSubject = "【活动在线网】【需求订阅】" + demand.Title;
                                var mailBody = GetEmailSubscriptionContent(emailSubscriptionContentFormat, demand);
                                var mailEntity = StaticUtility.EmailManager.CreateMailMessage(matchedEmails, displayName, mailSubject, mailBody);
                                var response = StaticUtility.EmailManager.Send(mailEntity);

                                StringBuilder logBuilder = new StringBuilder();
                                foreach (var item in mailEntity.To)
                                {
                                    logBuilder.Append(" 邮箱帐号: ").Append(item.Address).Append(", ");
                                }

                                if (response.IsSuccess)
                                {
                                    isEmailPushSuccess = true;
                                    LogService.LogWexin("邮箱推送需求成功", logBuilder.ToString());
                                }
                                else
                                {
                                    logBuilder.Append("失败原因为: ").Append(response.InnerException.ToString())
                                        .Append("\r\n");
                                    LogService.LogWexin("邮箱推送需求失败", logBuilder.ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                LogService.LogWexin("邮箱推送需求失败", ex.ToString());
                            }
                        }

                        #endregion

                        #region 生成推送历史

                        var pushHistories = new List<DemandSubscriptionPushHistory>();

                        foreach (var item in matchedSubscriptions)
                        {
                            pushHistories.Add(new DemandSubscriptionPushHistory()
                            {
                                CreatedDateTime = DateTime.Now,
                                DemandId = demand.Id,
                                EmailAddress = item.EmailAddress,
                                EmailStatus = isEmailPushSuccess ? "邮箱推送成功" : "邮箱推送失败",
                                IsMailSubscribed = item.IsEnableEmailSubscription.GetValueOrDefault(),
                                OpenId = item.OpenId,
                                WechatUserId = item.WeChatUserId
                            });
                        }

                        #endregion

                        #region 推送模板消息

                        if (matchOpenIds.Any())
                        {
                            var viewUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain +
                                          "/wechat/demand/show/" + demandId;
                            var messageData = new
                            {
                                first = new TemplateDataItem("您有一条新的业务信息，请查看"),
                                keyword1 = new TemplateDataItem(demand.ResourceTypeName),
                                keyword2 = new TemplateDataItem(demand.Title),
                                //keyword3 = new TemplateDataItem(demand.UserName),
                                keyword3 = new TemplateDataItem("活动在线网会员"),
                                keyword4 = new TemplateDataItem("点击详情立即查看"),
                                keyword5 = new TemplateDataItem(demand.InsertTime.ToString("yyyy-MM-dd HH:mm:ss")),
                                remark = new TemplateDataItem("\r\n点击详情立即查看。")
                            };

                            foreach (var openId in matchOpenIds)
                            {
                                var isSuccess = WeChatClient.Sender.SendTemplateMessage(openId,
                                    WeChatClient.Constant.TemplateMessage.DemandRemind, messageData, viewUrl);
                                var history = pushHistories.FirstOrDefault(item => item.OpenId == openId);
                                if (history != null)
                                {
                                    history.WechatStatus = isSuccess ? "微信推送成功" : "微信推送失败";
                                }
                            }
                        }

                        #endregion

                        #region 保存推送历史

                        subscriptionManager.AddSubscriptionPushHistories(pushHistories);

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogWexin("推送需求失败", ex.ToString());
                }
            });
        }

        /// <summary>
        /// {0}:需求标题， {1}：立即查看，{2}：需求类型，{3}：需求类别，{4}：需求预算，{5}：参与人数，
        /// {6}：开始时间，{7}：结束时间，{8}：立即查看，{9}：立即查看，{10}：立即查看，{11}：区域位置，{12}：需求详情
        /// </summary>
        /// <param name="emailSubscriptionContentFormat"></param>
        /// <param name="demand"></param>
        /// <returns></returns>
        private string GetEmailSubscriptionContent(string emailSubscriptionContentFormat, Demand demand)
        {
            string resourceSubTypeName = HtmlHelperExtension.GetResourceSubTypeNameById(demand.ResourceType, demand.ResourceTypeId);
            string buget = (!demand.Budget.HasValue || demand.Budget.Value <= 0) ? "面议" : demand.Budget + "元";
            string startTime = demand.StartTime.HasValue ? demand.StartTime.Value.ToString("yyyy-MM-dd") : "不确定";
            string endTime = demand.EndTime.HasValue ? demand.EndTime.Value.ToString("yyyy-MM-dd") : "不确定";
            string peopleCount = !string.IsNullOrEmpty(demand.PeopleNumber) ? demand.PeopleNumber : "不确定";

            string location = StaticUtility.GetProvinceAndCityNameById(demand.Province, demand.City, demand.Area) + " " + demand.Address;
            string titleLink = @"<a href='http://" + StaticUtility.Config.Domain + "/demand/show/"
                + demand.Id + "' target='_blank' style='color:#000;font-weight:bold;text-decoration:none;' title='"
                + demand.Title + "'>" + demand.Title + "</a>";
            string viewLink = @"<a href='http://" + StaticUtility.Config.Domain + "/demand/show/"
                + demand.Id + "' target='_blank' title='立即查看' style='color:#FF9900;font-weight:bold;text-decoration:none;'>立即查看</a>";

            return string.Format(emailSubscriptionContentFormat, titleLink, viewLink, demand.ResourceTypeName, resourceSubTypeName, buget, peopleCount,
                startTime, endTime, viewLink, viewLink, viewLink, location, demand.ContentStyle);
        }

        private bool IsDemandMatchWithSubscription(DemandSubscription subscription, Demand demand)
        {
            bool isLocationMatch = false;
            bool isDemandTypeMatch = false;
            bool isKeywordMatch = false;
            bool isBudgetMatch = false;

            if (subscription == null || demand == null)
            {
                return false;
            }

            var subscribedLocations = new List<Location>();
            var subscribedTypes = new List<DemandType>();
            var subscribedKeywords = new List<Keyword>();
            var subscribedBudgetCondition = "-1"; // 不限制

            // 获取用户订阅设置详情
            foreach (DemandSubscriptionDetail item in subscription.SubscriptionDetails)
            {
                if (item.SubscriptionType.Equals(DemandSubscriptionType.Area.ToString(),
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    subscribedLocations.Add(new Location(item.SubscriptionValue));
                }
                else if (item.SubscriptionType.Equals(DemandSubscriptionType.Category.ToString(),
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    subscribedTypes.Add(new DemandType(item.SubscriptionValue));
                }
                else if (item.SubscriptionType.Equals(DemandSubscriptionType.Keywords.ToString(),
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    subscribedKeywords.Add(new Keyword(item.SubscriptionValue));
                }
                else if (item.SubscriptionType.Equals(DemandSubscriptionType.Budget.ToString(),
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    subscribedBudgetCondition = item.SubscriptionValue;
                }
            }

            // 预算金额是否符合
            if (subscribedBudgetCondition.Equals("-1", StringComparison.CurrentCultureIgnoreCase))
            {
                isBudgetMatch = true;
            }
            else
            {
                using (var demandTable = new DataTable("Demand"))
                {
                    demandTable.Columns.Add("Budget", typeof(int));
                    var row = demandTable.NewRow();
                    row[0] = demand.Budget;
                    demandTable.Rows.Add(row);

                    var matchedDemand = demandTable.Select(subscribedBudgetCondition);
                    if (matchedDemand.Any())
                    {
                        isBudgetMatch = true;
                    }
                }
            }

            if (!isBudgetMatch)
            {
                return false;
            }

            // 区域位置是否符合
            if (subscribedLocations.Count == 0)
            {
                isLocationMatch = true;
            }
            else
            {
                foreach (var location in subscribedLocations)
                {
                    if (!location.IsMatch(demand.Province, demand.City, demand.Area)) continue;
                    isLocationMatch = true;
                    break;
                }
            }

            if (!isLocationMatch)
            {
                return false;
            }

            // 需求类型是否符合
            if (subscribedTypes.Count == 0)
            {
                isDemandTypeMatch = true;
            }
            else
            {
                foreach (var type in subscribedTypes)
                {
                    if (type.IsMatch(demand.ResourceType, demand.ResourceTypeId))
                    {
                        isDemandTypeMatch = true;
                        break;
                    }
                }
            }

            if (!isDemandTypeMatch)
                return false;

            // 关键字订阅是否符合
            if (subscribedKeywords.Count == 0)
            {
                isKeywordMatch = true;
            }
            else
            {
                foreach (var keyword in subscribedKeywords)
                {
                    if (!keyword.IsMatch(demand.Title))
                        continue;
                    isKeywordMatch = true;
                    break;
                }
            }

            return isKeywordMatch;
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

        internal class Location
        {
            private string province;
            private string city;
            private string area;

            internal Location(string location)
            {
                string[] locations = location.Split('_');
                if (locations.Length == 1)
                {
                    this.province = locations[0];
                    city = string.Empty;
                    area = string.Empty;
                }
                else if (locations.Length == 2)
                {
                    this.province = locations[0];
                    this.city = locations[1];
                    area = string.Empty;
                }
                else if (locations.Length == 3)
                {
                    this.province = locations[0];
                    this.city = locations[1];
                    this.area = locations[2];
                }
            }

            internal bool IsMatch(string province, string city, string area)
            {
                bool isMatch = false;

                //(loc.Province = demand.Province 
                // AND (loc.City = demand.City OR loc.City IS NULL OR loc.City = '') 
                // AND (loc.Area = demand.Area OR loc.Area IS NULL OR loc.Area = '')) 

                if (this.province.Equals(province, StringComparison.CurrentCultureIgnoreCase) &&
                    (this.city.Equals(city, StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty(this.city)) &&
                    (this.area.Equals(area, StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty(this.area)))
                {
                    isMatch = true;
                }

                return isMatch;
            }
        }

        internal class DemandType
        {
            private string resourceType;
            private string resourceSubType;

            internal DemandType(string subscriedType)
            {
                string[] types = subscriedType.Split('_');
                if (types.Length == 1)
                {
                    this.resourceType = types[0];
                    this.resourceSubType = string.Empty;
                }
                else if (types.Length == 2)
                {
                    this.resourceType = types[0];
                    this.resourceSubType = types[1];
                }
            }

            internal bool IsMatch(int resourceType, int? resourceSubType)
            {
                bool isMatch = false;
                //(cate.TypeId = demand.ResourceType 
                // AND (cate.SubTypeId IS NULL OR cate.SubTypeId = -1 OR cate.SubTypeId = demand.ResourceTypeId))
                if (this.resourceType.Equals(resourceType.ToString()) &&
                    (this.resourceSubType.Equals(resourceSubType.ToString()) || this.resourceSubType.Equals("-1") || string.IsNullOrEmpty(this.resourceSubType)))
                {
                    isMatch = true;
                }

                return isMatch;
            }
        }

        internal class Keyword
        {
            private string keyword;

            internal Keyword(string keyword)
            {
                this.keyword = keyword;
            }

            internal bool IsMatch(string source)
            {
                //(CHARINDEX(words.Keyword, demand.Title, 0) > 0 OR words.Keyword IS NULL OR words.Keyword = '')
                return string.IsNullOrEmpty(this.keyword) || source.Contains(this.keyword);
            }
        }
    }
}