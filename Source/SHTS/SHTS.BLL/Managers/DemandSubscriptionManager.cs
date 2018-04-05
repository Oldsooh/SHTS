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
        /// <returns></returns>
        public List<DemandSubscription> GetSubscriptions(int pageSize, int pageIndex, out int totalSubscriptionCount)
        {
            var subscriptions = new List<DemandSubscription>();
            totalSubscriptionCount = 0;

            try
            {
                var tempResult = subscriptionRepository.FindPage(pageSize, pageIndex, out totalSubscriptionCount,
                    (x => true), (x => x.SubscriptionId), false);

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

    }
}
