using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;
using Witbird.SHTS.Common;
using System.Transactions;
using Witbird.SHTS.DAL;

namespace Witbird.SHTS.BLL.Service
{
    /// <summary>
    /// 微信订阅业务逻辑处理类
    /// </summary>
    public class DemandSubscriptionService
    {
        #region Constants

        DemandSubscriptionRepository subscriptionRepository = new DemandSubscriptionRepository();
        DemandSubscriptionDetailRepository subscriptionDetailRepository = new DemandSubscriptionDetailRepository();

        #endregion Constants

        #region Public methods

        /// <summary>
        /// 获取用户微信订阅详细信息
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<DemandSubscription> GetSubscriptions(int pageSize, int pageIndex, out int totalSubscriptionCount)
        {
            var subscriptions = new List<DemandSubscription>();
            totalSubscriptionCount = 0;

            try
            {
                var tempResult = subscriptionRepository.FindPage(pageSize, pageIndex, out totalSubscriptionCount,
                    (x => true), (x => x.SubscriptionId), true);

                if (tempResult.HasItem())
                {
                    subscriptions = tempResult.ToList();

                    // 获取订阅详细信息
                    foreach (var item in subscriptions)
                    {
                        if (item.IsNotNull())
                        {
                            var details = subscriptionDetailRepository.FindAll(
                                (x => x.SubscriptionId == item.SubscriptionId), (x => x.InsertedTimestamp), true);
                            if (details.HasItem())
                            {
                                item.SubscriptionDetails.AddRange(details);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("获取用户微信订阅详细信息", ex.ToString());
            }

            return subscriptions;
        }

        /// <summary>
        /// 获取已订阅需求推送的用户详细订阅信息
        /// </summary>
        /// <returns></returns>
        public List<DemandSubscription> GetSubscriptionsOnlySubscribed()
        {
            var subscriptions = new List<DemandSubscription>();

            try
            {
                var tempResult = subscriptionRepository.FindAll(
                    (x => x.IsSubscribed), (x => x.SubscriptionId), true);

                if (tempResult.HasItem())
                {
                    subscriptions = tempResult.ToList();

                    // 获取订阅详细信息
                    foreach (var item in subscriptions)
                    {
                        if (item.IsNotNull() && item.IsSubscribed)
                        {
                            var details = subscriptionDetailRepository.FindAll(
                                (x => x.SubscriptionId == item.SubscriptionId), (x => x.InsertedTimestamp), true);
                            if (details.HasItem())
                            {
                                item.SubscriptionDetails.AddRange(details);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("获取已订阅需求推送的用户详细订阅信息", ex.ToString());
            }

            return subscriptions;
        }

        /// <summary>
        /// 根据用户ID获取用户需求订阅信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DemandSubscription GetSubscription(int userId)
        {
            var subcription = new DemandSubscription();

            try
            {
                var tempResult = subscriptionRepository.FindOne(x => x.WeChatUserId == userId);

                if (tempResult.IsNotNull())
                {
                    var details = subscriptionDetailRepository.FindAll(
                        (x => x.IsNotNull() && x.SubscriptionId == tempResult.SubscriptionId), (x => x.InsertedTimestamp), true);

                    if (details.HasItem())
                    {
                        tempResult.SubscriptionDetails.AddRange(details);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("根据用户ID获取用户需求订阅信息", ex.ToString());
            }

            return subcription;
        }

        /// <summary>
        /// 更新用户需求订阅信息
        /// </summary>
        /// <param name="subcription"></param>
        /// <returns>True: 更新成功，否则失败</returns>
        public bool UpdateSubscription(DemandSubscription subscription)
        {
            var isSuccessful = false;
            subscription.CheckNullObject("DemandSubscription");

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var temp = subscriptionRepository.FindOne(x => x.SubscriptionId == subscription.SubscriptionId);
                    if (temp.IsNotNull())
                    {
                        temp.IsSubscribed = subscription.IsSubscribed;
                        temp.LastUpdatedTimestamp = DateTime.Now;

                        // Deletes old subscription details first
                        DeleteSubscritpionDetails(subscription.SubscriptionId);

                        if (subscription.SubscriptionDetails.HasItem())
                        {
                            foreach (var item in subscription.SubscriptionDetails)
                            {
                                if (item.IsNotNull())
                                {
                                    item.SubscriptionId = subscription.SubscriptionId;
                                    item.InsertedTimestamp = DateTime.Now;
                                    subscriptionDetailRepository.AddEntity(item);
                                }
                            }
                        }
                    }

                    isSuccessful = subscriptionRepository.SaveChanges() > 0;
                    isSuccessful = isSuccessful && subscriptionDetailRepository.SaveChanges() > 0;
                    if (isSuccessful)
                    {
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("更新用户需求订阅信息并返回更新后的结果", ex.ToString());
            }

            return isSuccessful;
        }

        /// <summary>
        /// 更新最后推送时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLastPushTimestamp(int userId)
        {
            var isSuccessful = false;

            try
            {
                var subscription = subscriptionRepository.FindOne(x => x.WeChatUserId == userId);
                if (subscription.IsNotNull())
                {
                    subscription.LastPushTimestamp = DateTime.Now;
                    subscription.LastUpdatedTimestamp = subscription.LastPushTimestamp.Value;
                    isSuccessful = subscriptionRepository.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                LogService.Log("更新最后推送时间", ex.ToString());
            }

            return isSuccessful;
        }

        /// <summary>
        /// 更新最后推送时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLastPushTimestamp(List<int> userIds)
        {
            var isSuccessful = false;
            userIds.CheckNullObject("UserIds");

            try
            {
                if (userIds.HasItem())
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        var pushTimestamp = DateTime.Now;
                        foreach (var userId in userIds)
                        {
                            var subscription = subscriptionRepository.FindOne(x => x.WeChatUserId == userId);
                            if (subscription.IsNotNull())
                            {
                                subscription.LastPushTimestamp = pushTimestamp;
                                subscription.LastUpdatedTimestamp = pushTimestamp;
                            }
                        }

                        if (subscriptionRepository.SaveChanges() == userIds.Count)
                        {
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("更新最后推送时间", ex.ToString());
            }

            return isSuccessful;
        }

        /// <summary>
        /// 更新最后主动请求交互时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLastRequestTimestamp(int userId)
        {
            var isSuccessful = false;

            try
            {
                var subscription = subscriptionRepository.FindOne(x => x.WeChatUserId == userId);
                if (subscription.IsNotNull())
                {
                    subscription.LastRequestTimestamp = DateTime.Now;
                    subscription.LastUpdatedTimestamp = subscription.LastPushTimestamp.Value;
                    isSuccessful = subscriptionRepository.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                LogService.Log("更新最后主动请求交互时间", ex.ToString());
            }

            return isSuccessful;
        }

        /// <summary>
        /// 添加默认的需求订阅信息当新用户关注时
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool AddDefautlSubcription(int userId)
        {
            var isSuccessful = false;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var demandCatogoryRepository = new DemandCategoryRepository();
                    var catogroies = demandCatogoryRepository.FindAll();
                    catogroies.CheckNullObject("catogroies");

                    var currentTime = DateTime.Now;
                    var subscription = new DemandSubscription()
                    {
                        InsertedTimestamp = currentTime,
                        IsSubscribed = true,
                        LastPushTimestamp = currentTime,
                        LastRequestTimestamp = currentTime,
                        LastUpdatedTimestamp = currentTime,
                        WeChatUserId = userId
                    };

                    if (subscriptionRepository.AddEntitySave(subscription))
                    {
                        subscription = subscriptionRepository.FindOne(x => x.WeChatUserId == userId);
                        if (subscription.IsNotNull() && catogroies.HasItem())
                        {
                            foreach (var item in catogroies)
                            {
                                var subscriptionDetail = new DemandSubscriptionDetail()
                                {
                                    InsertedTimestamp = currentTime,
                                    SubscriptionId = subscription.SubscriptionId,
                                    SubscriptionType = DemandSubscriptionType.Category.ToString(),
                                    SubscriptionValue = item.Id.ToString()
                                };

                                subscriptionDetailRepository.AddEntity(subscriptionDetail);
                            }

                            isSuccessful = subscriptionDetailRepository.SaveChanges() > 0;
                        }
                    }

                    if (isSuccessful)
                    {
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("添加默认的需求订阅信息当新用户关注时", ex.ToString());
            }
            return isSuccessful;
        }

        #endregion Public methods


        #region Private methods

        /// <summary>
        /// 当用户更新订阅信息时，先删除老订阅信息，然后在添加
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool DeleteSubscritpionDetails(int subscriptionId)
        {
            var isSucessful = false;

            var details = subscriptionDetailRepository.FindAll(x => x.SubscriptionId == subscriptionId, x => x.InsertedTimestamp, true);
            if (details.HasItem())
            {
                foreach (var item in details)
                {
                    subscriptionDetailRepository.DeleteEntity(item);
                }

                isSucessful = subscriptionDetailRepository.SaveChanges() > 0;
            }

            return isSucessful;
        }

        #endregion Private methods

    }
}
