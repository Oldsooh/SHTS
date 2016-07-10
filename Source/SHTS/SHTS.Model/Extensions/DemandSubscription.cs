using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model
{
    public partial class DemandSubscription
    {
        private List<DemandSubscriptionDetail> subscriptionDetails = new List<DemandSubscriptionDetail>();

        /// <summary>
        /// 订阅用户微信昵称
        /// </summary>
        public string WeChatUserName { get; set; }

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
    }
}
