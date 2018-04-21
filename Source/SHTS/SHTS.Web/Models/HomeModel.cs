using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class HomeModel
    {
        /// <summary>
        /// 首页新闻列表
        /// </summary>
        public List<SinglePage> Newses { get; set; }

        /// <summary>
        /// 首页需求列表
        /// </summary>
        public List<Demand> Demands { get; set; }

        /// <summary>
        /// 首页活动列表
        /// </summary>
        public List<Activity> ActivityList { get; set; }

        /// <summary>
        /// 首页中介列表
        /// </summary>
        public List<Trade> Trades { get; set; }
    }
}