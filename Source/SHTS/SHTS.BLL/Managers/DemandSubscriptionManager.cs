using System;
using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;
using Witbird.SHTS.Common;

namespace Witbird.SHTS.BLL.Managers
{
    public class DemandSubscriptionManager
    {
        DemandSubscriptionRepository subscriptionRepository = new DemandSubscriptionRepository();
        DemandSubscriptionDetailRepository subscriptionDetailRepository = new DemandSubscriptionDetailRepository();

        private DemandSubscriptionPushHistoryRepository pushHistoryRepository =
            new DemandSubscriptionPushHistoryRepository();

        /// <summary>
        /// 添加推送历史记录
        /// </summary>
        /// <param name="histories"></param>
        public void AddSubscriptionPushHistories(List<DemandSubscriptionPushHistory> histories)
        {
            try
            {
                foreach (var history in histories)
                {
                    pushHistoryRepository.AddEntity(history);
                }

                pushHistoryRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                LogService.Log("保存推送历史记录失败", ex.ToString());
            }
        }

        /// <summary>
        /// 获取推送历史记录
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<DemandSubscriptionPushHistory> GetSubscriptionPushHistories(int pageSize, int pageIndex,
            out int totalCount, List<int> filterDemandIdList, string wechatUserNickName, string wechatStatus, string emailStatus)
        {
            totalCount = 0;
            List<DemandSubscriptionPushHistory> histories = new List<DemandSubscriptionPushHistory>();

            try
            {
                var context = DemandSubscriptionPushHistoryRepository.GetDbContext();
                
                // Selects the total data count out.
                totalCount = context.DemandSubscriptionPushHistory.Where
                    (
                        // History entity to be filterd
                        (item) =>
                        // Filters by demandId list
                        (!filterDemandIdList.Any() || filterDemandIdList.Contains(item.DemandId)) &&
                        // Filters by wechatStatus
                        (wechatStatus == "" || item.WechatStatus.Equals(wechatStatus, StringComparison.CurrentCultureIgnoreCase)) &&
                        // Filters by emailStatus
                        (emailStatus == "" || item.EmailStatus.Equals(emailStatus, StringComparison.CurrentCultureIgnoreCase))
                    )
                    // select wechat user information
                    .Join(context.WeChatUser, history => history.WechatUserId, wechatUser => wechatUser.Id,
                    (history, wechatUser) => new
                    {
                        history,
                        WechatUserName = wechatUser.NickName,
                        wechatUser.UserId
                    })
                    // Filters by wechatUserNickName
                    .Where(item => wechatUserNickName == "" || item.WechatUserName.IndexOf(wechatUserNickName) != -1)
                    .Count();

                // Selects detail history joined with wechatuser, user and demand informaion
                var temp = context.DemandSubscriptionPushHistory.Where
                    (
                        // History entity to be filterd
                        (item) =>
                        // Filters by demandId list
                        (!filterDemandIdList.Any() || filterDemandIdList.Contains(item.DemandId)) &&
                        // Filters by wechatStatus
                        (wechatStatus == "" || item.WechatStatus.Equals(wechatStatus, StringComparison.CurrentCultureIgnoreCase)) &&
                        // Filters by emailStatus
                        (emailStatus == "" || item.EmailStatus.Equals(emailStatus, StringComparison.CurrentCultureIgnoreCase))
                    )
                    // select wechat user information
                    .Join(context.WeChatUser, history => history.WechatUserId, wechatUser => wechatUser.Id,
                    (history, wechatUser) => new
                    {
                        history,
                        WechatUserName = wechatUser.NickName,
                        wechatUser.UserId
                    })
                    // Filters by wechatUserNickName
                    .Where(item => wechatUserNickName == "" || item.WechatUserName.IndexOf(wechatUserNickName) != -1)

                    .OrderByDescending(item => item.history.CreatedDateTime)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                    // selects user information
                    .GroupJoin(context.User, history => history.UserId, user => user.UserId,
                    (history, users) => new
                    {
                        history,
                        UserId = (users.Count() > 0) ? users.FirstOrDefault().UserId : -1,
                        UserName = (users.Count() > 0) ? users.FirstOrDefault().UserName : ""
                    })
                    // select demand information
                    .GroupJoin(context.Demand, history => history.history.history.DemandId, demand => demand.Id,
                    (history, demands) => new
                    {
                        history.history.history.Id,
                        history.history.history.DemandId,
                        history.history.history.CreatedDateTime,
                        DemandTitle = (demands.Count() > 0) ? demands.FirstOrDefault().Title : "",
                        history.history.history.EmailAddress,
                        history.history.history.EmailExceptionMessage,
                        history.history.history.EmailStatus,
                        history.history.history.IsMailSubscribed,
                        history.history.history.OpenId,
                        history.UserId,
                        history.UserName,
                        history.history.history.WechatExceptionMessage,
                        history.history.history.WechatStatus,
                        history.history.history.WechatUserId,
                        history.history.WechatUserName
                    });

                foreach (var item in temp)
                {
                    histories.Add(new DemandSubscriptionPushHistory()
                    {
                        Id = item.Id,
                        DemandId = item.DemandId,
                        CreatedDateTime = item.CreatedDateTime,
                        DemandTitle = item.DemandTitle,
                        EmailAddress = item.EmailAddress,
                        EmailExceptionMessage = item.EmailExceptionMessage,
                        EmailStatus = item.EmailStatus,
                        IsMailSubscribed = item.IsMailSubscribed,
                        OpenId = item.OpenId,
                        UserId = item.UserId,
                        UserName = item.UserName,
                        WechatExceptionMessage = item.WechatExceptionMessage,
                        WechatStatus = item.WechatStatus,
                        WechatUserId = item.WechatUserId,
                        WechatUserName = item.WechatUserName
                    });
                }
            }
            catch (Exception ex)
            {
                LogService.Log("获取需求推送历史记录失败", ex.ToString());
            }

            return histories;
        }

        /// <summary>
        /// 获取用户微信订阅详细信息
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalSubscriptionCount"></param>
        /// <returns></returns>
        public List<DemandSubscription> GetSubscriptions(int pageSize, int pageIndex, out int totalSubscriptionCount)
        {
            var subscriptions = new List<DemandSubscription>();
            totalSubscriptionCount = 0;

            try
            {
                var tempResult = subscriptionRepository.GetSubscriptions(pageSize, pageIndex, out totalSubscriptionCount);
                subscriptions.AddRange(tempResult);

                //if (subscriptions.HasItem())
                //{
                //    // 获取订阅详细信息
                //    foreach (var item in subscriptions)
                //    {
                //        if (item.IsNotNull())
                //        {
                //            var details = subscriptionDetailRepository.FindAll(
                //                (x => x.SubscriptionId == item.SubscriptionId), (x => x.InsertedTimestamp), true);
                //            if (details.HasItem())
                //            {
                //                item.SubscriptionDetails.AddRange(details);
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogService.Log("获取用户微信订阅详细信息", ex.ToString());
            }

            return subscriptions;
        }

    }
}
