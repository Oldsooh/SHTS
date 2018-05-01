using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class SubscriptionModel : BaseModel
    {
        public List<DemandSubscription> Subscriptions { get; } = new List<DemandSubscription>();

        public List<DemandSubscriptionPushHistory> PushHistories { get; } = new List<DemandSubscriptionPushHistory>();

        public List<int> FilterDemandIdList { get; set; } = new List<int>();
        public string FilterWechatUserNickName { get; set; } = "";
        public string FilterWechatStatus { get; set; } = "";
        public string FilterEmailStatus { get; set; } = "";
        public int FilterResourceType { get; set; }
        public int FilterSubResourceType { get; set; }
        public string FilterBudget { get; set; }

        public string FilterDemandIdString => $"demandIds={string.Join(",", FilterDemandIdList)}";
        public string FilterString => $"{FilterDemandIdString}&wechatUserNickName={FilterWechatUserNickName}&wechatStatus={FilterWechatStatus}&emailStatus={FilterEmailStatus}";

        public string FilterStringForSubscription => "resourceTypeId=" + FilterResourceType +
                "&subResourceTypeId=" + FilterSubResourceType +
                "&province=" + Province +
                "&city=" + City +
                "&area=" + Area +
                "&emailStatus=" + FilterEmailStatus +
                "&keywords=" + Keywords +
                "&wechatUserNickName=" + FilterWechatUserNickName +
                "&budgetCondition=" + FilterBudget;
    }
}