using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class SubscriptionModel : BaseModel
    {
        public List<DemandSubscription> Subscriptions { get; } = new List<DemandSubscription>();

        public List<DemandSubscriptionPushHistory> PushHistories { get; } = new List<DemandSubscriptionPushHistory>();
    }
}