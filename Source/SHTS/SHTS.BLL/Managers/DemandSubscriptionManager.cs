using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            out int totalCount)
        {
            totalCount = 0;
            var histories = new List<DemandSubscriptionPushHistory>();

            try
            {
                histories = pushHistoryRepository.FindPage(pageSize, pageIndex, out totalCount, (x => true),
                    (x => x.Id), false).ToList();
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
