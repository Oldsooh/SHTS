using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class NewsModel : BaseModel
    {
        public SinglePage News { get; set; }

        /// <summary>
        /// 新闻列表
        /// </summary>
        public List<SinglePage> Newses { get; set; }

        /// <summary>
        /// 幻灯片
        /// </summary>
        public List<SinglePage> Slides { get; set; }

        /// <summary>
        /// 网站公告
        /// </summary>
        public List<SinglePage> Notices { get; set; }

        /// <summary>
        /// 公司新闻
        /// </summary>
        public List<SinglePage> Companys { get; set; }

        /// <summary>
        /// 行业新闻
        /// </summary>
        public List<SinglePage> Industrys { get; set; }

        /// <summary>
        /// 资源新闻
        /// </summary>
        public List<SinglePage> Resources { get; set; }

        /// <summary>
        /// 供求新闻
        /// </summary>
        public List<SinglePage> Supplydemands { get; set; }
    }
}