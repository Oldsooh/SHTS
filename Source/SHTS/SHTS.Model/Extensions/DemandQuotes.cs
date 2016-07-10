using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
