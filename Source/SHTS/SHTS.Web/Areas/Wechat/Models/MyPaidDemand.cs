using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class MyPaidDemand : BaseModel
    {
        public List<TradeOrder> PaidDemandOrders { get; set; }
    }
}