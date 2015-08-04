using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model
{
    public partial class Trade
    {
        public Int64 RowNumber { get; set; }
        public string CreatedUserName
        {
            get;
            set;
        }
        public string SellerName { get; set; }
        public string BuyerName { get; set; }

        public User Seller { get; set; }
        public User Buyer { get; set; }
        public List<UserBankInfo> BankInfos { get; set; }
        public List<TradeHistory> Histories { get; set; }
    }
}
