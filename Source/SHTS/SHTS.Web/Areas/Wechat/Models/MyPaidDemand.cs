using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class MyPaidDemand : BaseModel
    {
        public List<TradeOrder> PaidDemandOrders { get; set; }
    }
}