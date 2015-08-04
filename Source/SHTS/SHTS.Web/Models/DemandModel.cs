using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class DemandModel : BaseModel
    {
        public Demand Demand { get; set; }

        public List<Demand> Demands { get; set; }

        public DemandCategory DemandCategory { get; set; }

        public List<DemandCategory> DemandCategories { get; set; }

        public string Category { get; set; }

        public string StartBudget { get; set; }

        public string EndBudget { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        /// <summary>
        /// 是否为会员, 会员可能查询详情
        /// </summary>
        public bool IsMember { get; set; }

        /// <summary>
        /// 是否为VIP，VIP可以查询联系方式
        /// </summary>
        public bool IsVIP { get; set; }
    }
}