using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class DemandModel : Witbird.SHTS.Web.Models.DemandModel
    {
        /// <summary>
        /// 当前访问的微信用户是否购买了此需求的联系方式
        /// </summary>
        public bool HasCurrentWeChatUserBought { get; set; }
        /// <summary>
        /// 当前访问的微信用户是否分享了此需求
        /// </summary>
        public bool HasCurrentUserSharedWechat
        {
            get;
            set;
        }

        public List<TradeOrder> PaidDemandOrders { get; set; }

        public string ActionName { get; set; }

        public string RouteFilters { get; set; }

        public WechatParameters WechatShareParameters { get; set; }
    }

    public class WechatParameters
    {
        public string AppId;
        public string Timestamp;
        public string NonceStr;
        public string Title;
        public string Link;
        public string Signature;
    }
}