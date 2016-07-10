using System.Collections.Generic;
using System.Linq;

namespace Witbird.SHTS.Model
{
    public partial class DemandQuotes
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
        /// 需求标题
        /// </summary>
        public string DemandTitle { get; set; }

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
    }
}
