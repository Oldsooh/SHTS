using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Mobile.Models
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