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

    }
}
