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
    }
}