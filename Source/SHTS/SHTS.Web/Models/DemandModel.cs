using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class DemandModel : BaseModel
    {
        public Demand Demand { get; set; }

        public List<Demand> Demands { get; set; }

        //public DemandCategory DemandCategory { get; set; }

        //public List<DemandCategory> DemandCategories { get; set; }

        public string LastResourceType { get; set; }
        public string ResourceType { get; set; }
        public string ResourceTypeId { get; set; }

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

        /// <summary>
        /// 场地类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "场地类型设置不正确")]
        public string SpaceTypeId { get; set; }

        /// <summary>
        /// 演员类别
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "演员类别设置不正确")]
        public string ActorTypeId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "设备类型设置不正确")]
        public string EquipTypeId { get; set; }
        
        /// <summary>
        /// “其他资源”的类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "此类型不存在")]
        public string OtherTypeId { get; set; }
    }
}