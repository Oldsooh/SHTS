using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class DemandModel : BaseModel
    {
        public Demand Demand { get; set; }

        public List<Demand> Demands { get; set; }

        public DemandCategory demandCategory { get; set; }

        public List<DemandCategory> DemandCategories { get; set; }

        public List<City> Provinces { get; set; }

        /// <summary>
        /// 是否为会员, 会员可能查询详情
        /// </summary>
        //public bool IsMember { get; set; }

        /// <summary>
        /// 是否为VIP，VIP可以查询联系方式
        /// </summary>
        //public bool IsVIP { get; set; }

        /// <summary>
        /// 当前访问的微信用户是否购买了此需求的联系方式
        /// </summary>
        public bool HasCurrentWeChatUserBought { get; set; }
    }
}