using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitBird.Com.SearchEngine
{
    public enum SearchResultType
    {
        /// <summary>
        /// 活动场地资源
        /// </summary>
        SpaceResource,
        /// <summary>
        /// 演艺人员资源
        /// </summary>
        ActorResource,
        /// <summary>
        /// 活动设备资源
        /// </summary>
        EquipmentResource,
        /// <summary>
        /// 其他资源
        /// </summary>
        OtherResource,
        /// <summary>
        /// 需求信息
        /// </summary>
        Demand,
        /// <summary>
        /// 活动在线
        /// </summary>
        Activity
    }
}
