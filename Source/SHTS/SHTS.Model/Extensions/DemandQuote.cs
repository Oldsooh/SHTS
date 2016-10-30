using System.Collections.Generic;
using System.Linq;

namespace Witbird.SHTS.Model
{
    public partial class DemandQuote
    {
        List<DemandQuoteHistory> quoteHistories = new List<DemandQuoteHistory>();

        /// <summary>
        /// 报价用户微信昵称
        /// </summary>
        public string WeChatUserName { get; set; }
        
        /// <summary>
        /// 单条报价回复历史记录
        /// </summary>
        public List<DemandQuoteHistory> QuoteHistories
        {
            get
            {
                return this.quoteHistories;
            }
        }

        /// <summary>
        /// 需求信息
        /// </summary>
        public Demand Demand { get; set; }

        /// <summary>
        /// 获取该报价所有未读消息数目
        /// </summary>
        public int NotReadCommentsCount
        {
            get
            {
                return this.quoteHistories.Count(x => x != null && !x.HasRead);
            }
        }

        /// <summary>
        /// 报价的用户是否已经购买了需求的联系方式
        /// </summary>
        public bool HasWeChatUserBoughtForDemand
        {
            get;
            set;
        }
    }
}
