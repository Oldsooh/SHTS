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
        /// {0}: pageSize, {1} pageIndex * pageSize
        /// </summary>
        private const string SelectSqlFormat = @"
SELECT 
[Limit1].[SubscriptionId] AS [SubscriptionId], 
[Limit1].[EmailAddress] AS [EmailAddress], 
[Limit1].[InsertedTimestamp] AS [InsertedTimestamp], 
[Limit1].[IsEnableEmailSubscription] AS [IsEnableEmailSubscription], 
[Limit1].[IsSubscribed] AS [IsSubscribed], 
[Limit1].[LastPushTimestamp] AS [LastPushTimestamp], 
[Limit1].[LastUpdatedTimestamp] AS [LastUpdatedTimestamp], 
[Join1].[OpenId] AS [OpenId], 
[Join1].[UserId] AS [UserId], 
[Join1].[Id] AS [Id], 
[Join1].[NickName] AS [NickName], 
[Join3].[UserName] AS [UserName],
Join4.SubscriptionDetailId,
Join4.SubscriptionId AS DetailSubscriptionId,
Join4.SubscriptionType,
Join4.SubscriptionValue,
Join4.InsertedTimestamp,
filters.DisplayName
FROM    (SELECT TOP ({0}) [Extent1].[SubscriptionId] AS [SubscriptionId], [Extent1].[WeChatUserId] AS [WeChatUserId], [Extent1].[IsSubscribed] AS [IsSubscribed], [Extent1].[LastPushTimestamp] AS [LastPushTimestamp], [Extent1].[InsertedTimestamp] AS [InsertedTimestamp], [Extent1].[LastUpdatedTimestamp] AS [LastUpdatedTimestamp], [Extent1].[IsEnableEmailSubscription] AS [IsEnableEmailSubscription], [Extent1].[EmailAddress] AS [EmailAddress]
	FROM ( SELECT [Extent1].[SubscriptionId] AS [SubscriptionId], [Extent1].[WeChatUserId] AS [WeChatUserId], [Extent1].[IsSubscribed] AS [IsSubscribed], [Extent1].[LastPushTimestamp] AS [LastPushTimestamp], [Extent1].[InsertedTimestamp] AS [InsertedTimestamp], [Extent1].[LastUpdatedTimestamp] AS [LastUpdatedTimestamp], [Extent1].[IsEnableEmailSubscription] AS [IsEnableEmailSubscription], [Extent1].[EmailAddress] AS [EmailAddress], row_number() OVER (ORDER BY [Extent1].[SubscriptionId] DESC) AS [row_number]
		FROM [dbo].[DemandSubscription] AS [Extent1]
	)  AS [Extent1]
	WHERE [Extent1].[row_number] > {1}
	ORDER BY [Extent1].[SubscriptionId] DESC ) AS [Limit1]
INNER JOIN  (SELECT [Extent2].[Id] AS [Id], [Extent2].[UserId] AS [UserId], [Extent2].[OpenId] AS [OpenId], [Extent2].[NickName] AS [NickName]
	FROM   ( SELECT 1 AS X ) AS [SingleRowTable1]
	LEFT OUTER JOIN [dbo].[WeChatUser] AS [Extent2] ON 1 = 1 ) AS [Join1] ON [Limit1].[WeChatUserId] = [Join1].[Id]
LEFT JOIN  (SELECT [Extent3].[UserId] AS [UserId], [Extent3].[UserName] AS [UserName]
	FROM   ( SELECT 1 AS X ) AS [SingleRowTable2]
	LEFT OUTER JOIN [dbo].[User] AS [Extent3] ON 1 = 1 ) AS [Join3] ON ([Join1].[UserId] = [Join3].[UserId]) OR (([Join1].[UserId] IS NULL) AND ([Join3].[UserId] IS NULL))
LEFT JOIN [dbo].[DemandSubscriptionDetail] Join4 ON Join4.SubscriptionId = [Limit1].SubscriptionId
LEFT JOIN [dbo].[BudgetFilters] filters ON Join4.SubscriptionType = N'Budget' and filters.Condition = Join4.SubscriptionValue
ORDER BY [Limit1].[SubscriptionId] DESC";

        public List<DemandSubscription> GetSubscriptions(int pageSize, int pageIndex, out int total)
        {
            var db = GetDbContext();
            total = db.DemandSubscription.Count();

            //var queryResult = db.DemandSubscription.OrderByDescending(item => item.SubscriptionId).Skip((pageIndex - 1) * pageSize).Take(pageSize)
            //    .Join(db.WeChatUser.DefaultIfEmpty(), subscritpion => subscritpion.WeChatUserId, wechatUser => wechatUser.Id,
            //        (subscription, wechatUser) => new
            //        {
            //            subscription.EmailAddress,
            //            subscription.InsertedTimestamp,
            //            subscription.IsEnableEmailSubscription,
            //            subscription.IsSubscribed,
            //            subscription.LastPushTimestamp,
            //            subscription.LastUpdatedTimestamp,
            //            wechatUser.OpenId,
            //            subscription.SubscriptionId,
            //            wechatUser.UserId,
            //            WechatUserId = wechatUser.Id,
            //            WeChatUserName = wechatUser.NickName
            //        }).Join(db.User.DefaultIfEmpty(), temp => temp.UserId.Value, user => user.UserId, (temp, user) => new
            //        {
            //            temp.EmailAddress,
            //            temp.InsertedTimestamp,
            //            temp.IsEnableEmailSubscription,
            //            temp.IsSubscribed,
            //            temp.LastPushTimestamp,
            //            temp.LastUpdatedTimestamp,
            //            temp.OpenId,
            //            temp.SubscriptionId,
            //            temp.UserId,
            //            temp.WechatUserId,
            //            temp.WeChatUserName,
            //            user.UserName
            //        });

            var subscriptions = new List<DemandSubscription>();

            using (var conn = DBHelper.GetSqlConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(SelectSqlFormat, pageSize, (pageIndex - 1) * pageSize);
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
                        if (reader[12] != DBNull.Value)
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
                }
            }

            return subscriptions;
        }
    }
}
