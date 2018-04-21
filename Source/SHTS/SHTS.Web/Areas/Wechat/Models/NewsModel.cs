using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class NewsModel : BaseModel
    {
        public SinglePage News { get; set; }

        /// <summary>
        /// 新闻列表
        /// </summary>
        public List<SinglePage> Newses { get; set; }
    }
}