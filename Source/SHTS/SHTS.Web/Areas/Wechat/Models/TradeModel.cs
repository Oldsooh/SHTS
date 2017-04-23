using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class WeChatTradeModel : TradeModel
    {
        public WeChatUser CurrentWeChatUser { get; set; }
        public TradeParameter TradeParameter { get; set; }
    }

    public class TradeParameter
    {
        public string TradeType { get; set; }
        public int TradeResourceId { get; set; }
        public string TradeRule { get; set; }
        public string TradeUserName { get; set; }
        public int TradeUserId { get; set; }
        public string TradeRelationShip { get; set; }
        public string TradeTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(TradeUserName))
                {
                    if (this.TradeRelationShip.Equals("buyer", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return "与卖家" + TradeUserName + "进行中介申请";
                    }
                    else
                    {
                        return "与买家" + TradeUserName + "进行中介申请";
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string TradeResourceUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(TradeType))
                {
                    var url = Fetch.BuildBaseUrl("/" + TradeType + "/show/" + TradeResourceId.ToString());
                    return url;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}