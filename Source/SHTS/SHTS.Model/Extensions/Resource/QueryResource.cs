using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model.Extensions
{
    public class QueryResource
    {
        /// <summary>
        /// 1
        /// 2
        /// 3
        /// 4
        /// </summary>
        public int ResourceType { get; set; }

        #region Common
        /// <summary>
        /// from 0
        /// </summary>
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        //public string ProvinceId { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// 商圈
        /// </summary>
        public string AreaId { get; set; }

        ///// <summary>
        ///// 结束时间
        ///// </summary>
        //public DateTime StartDate { get; set; }

        ///// <summary>
        ///// 开始时间
        ///// </summary>
        //public DateTime EndDate { get; set; }

        public int State { get; set; }

        /// <summary>
        /// 报价范围
        /// </summary>
        public string QuotePriceCondition { get; set; }
        #endregion

        #region Space
        /// <summary>
        /// 场地类型
        /// </summary>
        public int SpaceType { get; set; }

        /// <summary>
        /// 场地特点
        /// </summary>
        public int SpaceFeature { get; set; }

        /// <summary>
        /// 提供设备
        /// </summary>
        public int SpaceFacility { get; set; }

        /// <summary>
        /// 场地大小
        /// </summary>
        public int SpaceSizeId { get; set; }

        /// <summary>
        /// 容纳人数
        /// </summary>
        public int SpacePeopleId { get; set; }

        /// <summary>
        /// 是否提供宴请
        /// </summary>
        public int SpaceTreat { get; set; }
        #endregion

        #region Actor
        /// <summary>
        /// 对应演出类别
        /// </summary>
        public int ActorTypeId { get; set; }

        /// <summary>
        /// 1个人 2团体 3公司
        /// </summary>
        public int ActorFromId { get; set; }

        /// <summary>
        /// 1男 2女
        /// </summary>
        public int ActorSex { get; set; }
        #endregion

        #region Equip
        /// <summary>
        /// 设备类别
        /// </summary>
        public int EquipTypeId { get; set; }
        #endregion

        #region Other
        /// <summary>
        /// 其他资源的类别编号
        /// </summary>
        public int OtherTypeId { get; set; }
        #endregion

        #region User
        public int UserId { get; set; }
        #endregion
    }
}
