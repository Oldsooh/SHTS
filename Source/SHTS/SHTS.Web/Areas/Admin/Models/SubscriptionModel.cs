using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class SubscriptionModel : BaseModel
    {
        public List<DemandSubscription> Subscriptions { get; } = new List<DemandSubscription>();

        public List<DemandSubscriptionPushHistory> PushHistories { get; } = new List<DemandSubscriptionPushHistory>();

        public List<int> FilterDemandIdList { get; set; } = new List<int>();

        public string FilterDemandIdString => $"demandIds={string.Join(",", FilterDemandIdList)}";
    }
}