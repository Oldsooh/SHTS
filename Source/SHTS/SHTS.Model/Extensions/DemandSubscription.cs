using System.Collections.Generic;
using System.Linq;

namespace Witbird.SHTS.Model
{
    public partial class DemandSubscription
    {
        private List<DemandSubscriptionDetail> subscriptionDetails = new List<DemandSubscriptionDetail>();

        /// <summary>
        /// 订阅用户微信昵称
        /// </summary>
        public string WeChatUserName { get; set; }

        public string OpenId { get; set; }

        public int? UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// 用户订阅内容详细集合
        /// </summary>
        public List<DemandSubscriptionDetail> SubscriptionDetails
        {
            get
            {
                return this.subscriptionDetails;
            }
        }

        /// <summary>
        /// 获取用户订阅的需求类型
        /// </summary>
        public List<DemandSubscriptionDetail> SubscribedTypes
        {
            get
            {
                return this.subscriptionDetails.Where(x => x.SubscriptionType == DemandSubscriptionType.Category.ToString()).ToList();
            }
        }

        /// <summary>
        /// 获取用户订阅的区域位置
        /// </summary>
        public List<DemandSubscriptionDetail> SubscribedAreas
        {
            get
            {
                return this.subscriptionDetails.Where(x => x.SubscriptionType == DemandSubscriptionType.Area.ToString()).ToList();
            }
        }

        /// <summary>
        /// 获取用户订阅的关键字
        /// </summary>
        public List<DemandSubscriptionDetail> SubscribedKeywords
        {
            get
            {
                return this.subscriptionDetails.Where(x => x.SubscriptionType == DemandSubscriptionType.Keywords.ToString()).ToList();
            }
        }

        public List<DemandSubscriptionDetail> SubscribedBudgetConditions
        {
            get
            {
                return this.subscriptionDetails
                    .Where(x => x.SubscriptionType == DemandSubscriptionType.Budget.ToString()).ToList();
            }
        }

        public string SubscribedBudgetCondtionDisplayName
        {
            get
            {
                var budgetCondition = this.subscriptionDetails.FirstOrDefault(item =>
                    item.SubscriptionType == DemandSubscriptionType.Budget.ToString());
                if (budgetCondition != null)
                {
                    return budgetCondition.BudgetConditionDisplayName;
                }

                return string.Empty;
            }
        }
    }
}
