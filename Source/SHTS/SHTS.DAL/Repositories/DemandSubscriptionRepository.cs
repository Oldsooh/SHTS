using System;
using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL
{
    public class DemandSubscriptionRepository : BaseRepository<DemandSubscription>
    {
        /// <summary>
        /// {0}: pageSize, {1} pageIndex * pageSize, {2}: wherecondition for subscription
        /// </summary>
        private const string SelectSqlFormat = @"
SELECT subscription.[SubscriptionId] AS [SubscriptionId], subscription.[EmailAddress] AS [EmailAddress], subscription.[InsertedTimestamp] AS [SubscriptionInsertedTimestamp], subscription.[IsEnableEmailSubscription] AS [IsEnableEmailSubscription], subscription.[IsSubscribed] AS [IsSubscribed]
	, subscription.[LastPushTimestamp] AS [LastPushTimestamp], subscription.[LastUpdatedTimestamp] AS [LastUpdatedTimestamp], wechatUser.[OpenId] AS [OpenId], wechatUser.[UserId] AS [UserId], wechatUser.[Id] AS [Id]
	, wechatUser.[NickName] AS [NickName], [user].[UserName] AS [UserName], detail.SubscriptionDetailId, detail.SubscriptionId AS DetailSubscriptionId
	, detail.SubscriptionType, detail.SubscriptionValue, detail.InsertedTimestamp AS [DetailInsertedTimestamp]
	,filters.DisplayName
FROM (
	SELECT rowNum.SubscriptionId
	FROM (
		SELECT temp.[SubscriptionId], ROW_NUMBER() OVER (ORDER BY temp.[SubscriptionId] DESC) AS [row_number]
		FROM [DemandSubscription] temp
			INNER JOIN [dbo].[WeChatUser] wechatUser ON temp.[WeChatUserId] = wechatUser.[Id]
			LEFT JOIN [dbo].[DemandSubscriptionDetail] detail ON detail.SubscriptionId = temp.SubscriptionId
        WHERE {2}
		GROUP BY temp.[SubscriptionId]
	) rowNum
	WHERE rowNum.[row_number] > {0}
		AND rowNum.[row_number] <= {1}
) subscriptionIds
	INNER JOIN [DemandSubscription] subscription ON subscription.SubscriptionId = subscriptionIds.SubscriptionId
	LEFT JOIN [dbo].[DemandSubscriptionDetail] detail ON detail.SubscriptionId = subscription.SubscriptionId
	INNER JOIN [dbo].[WeChatUser] wechatUser ON subscription.[WeChatUserId] = wechatUser.[Id]
	LEFT JOIN [dbo].[User] [user] ON wechatUser.[UserId] = [user].[UserId]
	LEFT JOIN [dbo].[BudgetFilters] filters
	ON detail.SubscriptionType = N'Budget'
		AND filters.Condition = detail.SubscriptionValue
    ORDER BY subscription.SubscriptionId DESC
;

SELECT COUNT(1) FROM
(
SELECT DISTINCT subscription.SubscriptionId
FROM [DemandSubscription] subscription
	INNER JOIN [dbo].[WeChatUser] wechatUser ON subscription.[WeChatUserId] = wechatUser.[Id]
	LEFT JOIN [dbo].[DemandSubscriptionDetail] detail ON detail.SubscriptionId = subscription.SubscriptionId
WHERE {2} -- same where condition with above
GROUP BY subscription.SubscriptionId
) AS result
";

        public List<DemandSubscription> GetSubscriptions(int pageSize, int pageIndex, out int total, string whereCondition = "")
        {
            total = 0;
            
            var subscriptions = new List<DemandSubscription>();

            using (var conn = DBHelper.GetSqlConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(SelectSqlFormat, (pageIndex - 1) * pageSize, pageIndex * pageSize, whereCondition);
                cmd.CommandType = System.Data.CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var subscription = new DemandSubscription()
                        {
                            SubscriptionId = reader[0].DBToInt32(),
                            EmailAddress = reader[1].DBToString(),
                            InsertedTimestamp = reader[2].DBToDateTime().Value,
                            IsEnableEmailSubscription = reader[3].DBToBoolean(),
                            IsSubscribed = reader[4].DBToBoolean(),
                            LastPushTimestamp = reader[5].DBToDateTime(),
                            LastUpdatedTimestamp = reader[6].DBToDateTime().Value,
                            OpenId = reader[7].DBToString(),
                            UserId = reader[8].DBToNullableInt32(),
                            WeChatUserId = reader[9].DBToInt32(),
                            WeChatUserName = reader[10].DBToString(),
                            UserName = reader[11].DBToString()
                        };
                        
                        DemandSubscriptionDetail detail = null;
                        if (reader[12] != DBNull.Value) // SubscripitionDetailId
                        {
                            detail = new DemandSubscriptionDetail()
                            {
                                SubscriptionDetailId = reader[12].DBToInt32(),
                                SubscriptionId = reader[13].DBToInt32(),
                                SubscriptionType = reader[14].DBToString(),
                                SubscriptionValue = reader[15].DBToString(),
                                InsertedTimestamp = reader[16].DBToDateTime().Value,
                                BudgetConditionDisplayName = reader[17].DBToString()
                            };
                        }

                        var existSubscription =
                            subscriptions.FirstOrDefault(item => item.SubscriptionId == subscription.SubscriptionId);
                        if (existSubscription != null && detail != null)
                        {
                            existSubscription.SubscriptionDetails.Add(detail);
                        }
                        else
                        {
                            if (detail != null)
                            {
                                subscription.SubscriptionDetails.Add(detail);
                            }

                            subscriptions.Add(subscription);
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            total = reader[0].DBToInt32();
                            break;
                        }
                    }
                }
            }

            return subscriptions;
        }
    }
}
