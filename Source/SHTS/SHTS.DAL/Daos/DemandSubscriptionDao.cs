using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using System.Data.SqlClient;

namespace Witbird.SHTS.DAL.Daos
{
    public class DemandSubscriptionDao
    {
        #region Constants

        const string SP_DeleteDemandSubscriptionDetailsBySubscriptionId = "sp_DeleteDemandSubscriptionDetailsBySubscriptionId";
        const string SP_UpdateDemandSubscription = "sp_UpdateDemandSubscription";
        const string SP_InsertOrUpdateDemandSubscription = "sp_InsertOrUpdateDemandSubscription";
        const string SP_SelectDemandSubscriptionByWeChatUserId = "sp_SelectDemandSubscriptionByWeChatUserId";
        const string SP_SelectAllSubscriedSubscriptions = "sp_SelectAllSubscriedSubscriptions";
        const string SP_InsertDemandSubscriptionDetail = "sp_InsertDemandSubscriptionDetail";
        const string SP_UpdateDemandSubscriptionLastPushTime = "sp_UpdateDemandSubscriptionLastPushTime";

        const string Parameter_SubscriptionId = "@SubscriptionId";
        const string Parameter_WeChatUserId = "@WeChatUserId";
        const string Parameter_IsSubscribed = "@IsSubscribed";
        const string Parameter_LastRequestTimestamp = "@LastRequestTimestamp";
        const string Parameter_LastPushTimestamp = "@LastPushTimestamp";
        const string Parameter_InsertedTimestamp = "@InsertedTimestamp";
        const string Parameter_LastUpdatedTimestamp = "@LastUpdatedTimestamp";
        const string Parameter_SubscriptionDetailId = "@SubscriptionDetailId";
        const string Parameter_SubscriptionType = "@SubscriptionType";
        const string Parameter_SubscriptionValue = "@SubscriptionValue";
        const string Parameter_IsEnableEmailSubscription = "@IsEnableEmailSubscription";
        const string Parameter_EmailAddress = "@EmailAddress";

        #endregion

        #region Public Methods


        /// <summary>
        /// 查询所有已订阅详细信息
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<DemandSubscription> SelectAllSubscriedSubscriptionsWithDetails(SqlConnection conn)
        {
            var subscriptions = new List<DemandSubscription>();
            using (SqlDataReader reader = DBHelper.RunProcedure(conn, SP_SelectAllSubscriedSubscriptions, null))
            {
                while (reader.Read())
                {
                    var subscription = ConvertToDemandSubscription(reader);
                    subscriptions.Add(subscription);
                }

                reader.NextResult();

                while (reader.Read())
                {
                    var detail = ConvertToDemandSubscriptionDetail(reader);
                    var subscription = subscriptions.FirstOrDefault(x => x.SubscriptionId == detail.SubscriptionId);
                    if (subscription.IsNotNull() && !subscription.SubscriptionDetails.Contains(detail))
                    {
                        subscription.SubscriptionDetails.Add(detail);
                    }
                }
            }

            return subscriptions;
        }

        /// <summary>
        /// Selects subscription and details by wechat user id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DemandSubscription SelectSubscriptionByUserId(SqlConnection conn, int userId)
        {
            DemandSubscription subscription = null;
            SqlParameter[] parameters = new SqlParameter[] 
            {
                new SqlParameter(Parameter_WeChatUserId, userId)
            };

            using (var reader = DBHelper.RunProcedure(conn, SP_SelectDemandSubscriptionByWeChatUserId, parameters))
            {
                while(reader.Read())
                {
                    subscription = ConvertToDemandSubscription(reader);
                }

                if (subscription.IsNotNull() && reader.NextResult())
                {
                    while(reader.Read())
                    {
                        var subDetail = ConvertToDemandSubscriptionDetail(reader);
                        if (subDetail.IsNotNull())
                        {
                            subscription.SubscriptionDetails.Add(subDetail);
                        }
                    }
                }
            }

            return subscription;
        }

        /// <summary>
        /// Inserts or updates subscription information.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public DemandSubscription InsertOrUpdateSubscription(SqlConnection conn, DemandSubscription subscription)
        {
            ParameterChecker.Check(subscription, "DemandSubscription");

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_SubscriptionId, subscription.SubscriptionId),
                new SqlParameter(Parameter_WeChatUserId, subscription.WeChatUserId),
                new SqlParameter(Parameter_IsSubscribed, subscription.IsSubscribed),
                new SqlParameter(Parameter_LastPushTimestamp, subscription.LastPushTimestamp ?? subscription.LastUpdatedTimestamp),
                new SqlParameter(Parameter_LastUpdatedTimestamp, subscription.LastUpdatedTimestamp),
                new SqlParameter(Parameter_InsertedTimestamp, subscription.LastUpdatedTimestamp),
                new SqlParameter(Parameter_IsEnableEmailSubscription, subscription.IsEnableEmailSubscription),
                new SqlParameter(Parameter_EmailAddress, subscription.EmailAddress)
            };

            using (SqlDataReader reader = DBHelper.RunProcedure(conn, SP_InsertOrUpdateDemandSubscription, parameters))
            {
                while(reader.Read())
                {
                    subscription.SubscriptionId = reader[0].DBToInt32();
                }
            }

            return subscription;
        }

        /// <summary>
        /// Inserts subscription details.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="subscriptionDetail"></param>
        /// <returns></returns>
        public bool InsertSubscriptionDetail(SqlConnection conn, DemandSubscriptionDetail subscriptionDetail)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_SubscriptionId, subscriptionDetail.SubscriptionId),
                new SqlParameter(Parameter_SubscriptionType, subscriptionDetail.SubscriptionType),
                new SqlParameter(Parameter_SubscriptionValue, subscriptionDetail.SubscriptionValue),
                new SqlParameter(Parameter_InsertedTimestamp, subscriptionDetail.InsertedTimestamp)
            };

            return DBHelper.RunNonQueryProcedure(conn, SP_InsertDemandSubscriptionDetail, parameters) > 0;
        }

        /// <summary>
        /// Deletes subscription details by subscription id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="subscriptionId"></param>
        public void DeleteSubscriptionDetails(SqlConnection conn, int subscriptionId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_SubscriptionId, subscriptionId)
            };

            DBHelper.RunNonQueryProcedure(conn, SP_DeleteDemandSubscriptionDetailsBySubscriptionId, parameters);
        }

        public bool UpdateDemandSubscriptionLastPushTime(SqlConnection conn, int userId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_WeChatUserId, userId)
            };

            return DBHelper.RunNonQueryProcedure(conn, SP_UpdateDemandSubscriptionLastPushTime, parameters) > 0;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts object to DemandSubscription entity.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DemandSubscription ConvertToDemandSubscription(SqlDataReader reader)
        {
            return new DemandSubscription()
            {
                InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime().Value,
                IsSubscribed = reader["IsSubscribed"].DBToBoolean(),
                LastPushTimestamp = reader["LastPushTimestamp"].DBToDateTime(),
                LastUpdatedTimestamp = reader["LastUpdatedTimestamp"].DBToDateTime().Value,
                SubscriptionId = reader["SubscriptionId"].DBToInt32(),
                WeChatUserId = reader["WeChatUserId"].DBToInt32(),
                IsEnableEmailSubscription = reader["IsEnableEmailSubscription"].DBToBoolean(false),
                EmailAddress = reader["EmailAddress"].DBToString(),
                OpenId = reader["OpenId"].DBToString(),
                UserId = reader["UserId"].DBToNullableInt32()
            };
        }

        /// <summary>
        /// Converts object to DemandSubscriptionDetail entity.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DemandSubscriptionDetail ConvertToDemandSubscriptionDetail(SqlDataReader reader)
        {
            return new DemandSubscriptionDetail()
            {
                InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime().Value,
                SubscriptionDetailId = reader["SubscriptionDetailId"].DBToInt32(),
                SubscriptionId = reader["SubscriptionId"].DBToInt32(),
                SubscriptionType = reader["SubscriptionType"].DBToString(),
                SubscriptionValue = reader["SubscriptionValue"].DBToString()
            };
        }

        #endregion
    }
}
