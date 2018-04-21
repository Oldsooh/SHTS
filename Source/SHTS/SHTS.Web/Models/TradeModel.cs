using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class TradeModel : BaseModel
    {
        /// <summary>
        /// Gets or sets current logged on user information.
        /// </summary>
        public Model.User CurrentUser { get; set; }

        public PublicConfigModel TradeConfig { get; set; }

        /// <summary>
        /// Gets or sets user bank info list.
        /// </summary>
        public List<UserBankInfo> BankInfos { get; set; }

        public Trade CurrentTrade { get; set; }
        public List<Trade> TradeList { get; set; }

        public string Filter { get; set; }
    }
}