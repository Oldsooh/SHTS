using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class CityModel : SHTS.Web.Models.CityModel
    {
        public WechatParameters WechatParameters { get; set; }
    }
}