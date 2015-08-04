using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class OrderModel
    {
        public TradeOrder Order { get; set; }
        public string ReturnUrl { get; set; }

        public List<PublicConfig> OfflineBankInfos { get; set; }
    }
}