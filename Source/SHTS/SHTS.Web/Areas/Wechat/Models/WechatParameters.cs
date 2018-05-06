using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class WechatParameters
    {
        public string AppId { get; set; }
        public string Timestamp { get; set; }
        public string NonceStr { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public string Signature { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public string DataUrl { get; set; }
        public string CallbakUrl { get; set; }
    }
}