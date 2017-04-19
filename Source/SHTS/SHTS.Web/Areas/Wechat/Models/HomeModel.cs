using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class HomeModel
    {

        /// <summary>
        /// 首页需求列表
        /// </summary>
        public List<Demand> Demands { get; set; }

        /// <summary>
        /// 首页活动列表
        /// </summary>
        public List<Activity> ActivityList { get; set; }

        /// <summary>
        /// 右侧四大资源
        /// </summary>
        public Right Right { get; set; }
    }
}