using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;

namespace Witbird.SHTS.DAL.Daos
{
    public class ActivityDao
    {
        #region const

        //存储过程名称
        private const string sp_GetActivityById = "sp_GetActivityById";
        private const string SP_CreateOrUpdateActivity = "sp_CreateOrUpdateActivity";
        private const string sp_QueryActivitys = "sp_QueryActivitys";
        private const string sp_UpdateActivityStatu = "sp_UpdateActivityStatu";

        //列名
        private const string column_activityId = "@Id";

        #endregion

        #region 相关操作

        public Activity GetActivityById(SqlConnection conn, int activityId)
        {
            Activity activity = null;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(column_activityId, activityId)
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn, sp_GetActivityById, sqlParameters);
            while (reader.Read())
            {
                activity = ConvertToActivityObject(reader);
            }
            if (reader != null)
            {
                reader.Close();
            }
            return activity;
        }

        /// <summary>
        /// 发布活动。
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public bool CreateOrUpdateActivity(SqlConnection conn, Activity activity)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter(column_activityId, activity.Id),
				new SqlParameter("@ActivityType", activity.ActivityType),
				new SqlParameter("@Adress", activity.Adress),
				new SqlParameter("@ContentStyle", activity.ContentStyle),
				new SqlParameter("@ContentText", activity.ContentText),
				new SqlParameter("@Description", activity.Description),
				new SqlParameter("@EndTime", activity.EndTime),
				new SqlParameter("@HoldBy", activity.HoldBy),
				new SqlParameter("@ImageUrl", activity.ImageUrl),
				new SqlParameter("@Jingdu", activity.Jingdu),
				new SqlParameter("@Weidu", activity.Weidu),
				new SqlParameter("@Keywords", activity.Keywords),
                new SqlParameter("@Link", activity.Link),
                new SqlParameter("@StartTime", activity.StartTime),
                new SqlParameter("@State", activity.State),
                new SqlParameter("@Title", activity.Title),
                new SqlParameter("@UserId", activity.UserId),
                new SqlParameter("@ViewCount", activity.ViewCount ?? 0),
                new SqlParameter("@LocationId", activity.LocationId),
				new SqlParameter("@CreatedTime", DateTime.Now),
				new SqlParameter("@LastUpdatedTime", DateTime.Now),
                new SqlParameter("@IsFromMobile", activity.IsFromMobile)
           };
            DBHelper.CheckSqlSpParameter(parameters);
            return DBHelper.RunNonQueryProcedure(conn, SP_CreateOrUpdateActivity, parameters) > 0;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="Criteria"></param>
        /// <returns></returns>
        public List<Activity> QueryActivities(SqlConnection conn, QueryActivityCriteria Criteria)
        {
            List<Activity> activities = new List<Activity>();
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityType", Criteria.ActivityType),
                new SqlParameter("@EndTime", Criteria.EndTime),
                new SqlParameter("@PageSize", Criteria.PageSize),
                new SqlParameter("@QueryType", Criteria.QueryType),
                new SqlParameter("@StartRowIndex", Criteria.StartRowIndex),
                new SqlParameter("@StartTime", Criteria.StartTime),
                new SqlParameter("@cityid", Criteria.cityid),
                new SqlParameter("@UserId", Criteria.UserId),
                new SqlParameter("@Status", Criteria.Status),
                new SqlParameter("@LastUpdatedTime", Criteria.LastUpdatedTime),
                new SqlParameter("@keywors", Criteria.Keyword)
            };
            DBHelper.CheckSqlSpParameter(sqlParameters);
            SqlDataReader reader = DBHelper.RunProcedure(conn,
                sp_QueryActivitys, sqlParameters);
            while (reader.Read())
            {
                activities.Add(ConvertToActivityObject(reader));
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    Criteria.ResultTotalCount = reader[Constants.column_totalCount].DBToInt32();
                }
            }
            if (reader != null)
            {
                reader.Close();
            }
            return activities;
        }

        /// <summary>
        /// 更新activity 狀態
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public bool UpdateActivityStatu(SqlConnection conn, Activity activity)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter(column_activityId, activity.Id),
                new SqlParameter("@State", activity.State)
           };
            DBHelper.CheckSqlSpParameter(parameters);
            return DBHelper.RunNonQueryProcedure(conn, sp_UpdateActivityStatu, parameters) > 0;
        }

        public bool InsertOrUpdateActivityVote(SqlConnection conn, ActivityVote vote)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter("@VoteId", vote.VoteId),
                new SqlParameter("@UserId", vote.UserId),
                new SqlParameter("@WechatUserOpenId", vote.WechatUserOpenId),
                new SqlParameter("@ActivityId", vote.ActivityId),
                new SqlParameter("@IsVoted", vote.IsVoted),
                new SqlParameter("@InsertedTimestamp", vote.InsertedTimestamp),
                new SqlParameter("@LastUpdatedTimestamp", vote.LastUpdatedTimestamp)
            };

            DBHelper.CheckSqlSpParameter(parameters);

            return DBHelper.RunNonQueryProcedure(conn, "sp_InsertOrUpdateActivityVote", parameters) > 0;
        }

        public ActivityVote SelectActivityVoteByUserId(SqlConnection conn, int activityId, int userId)
        {
            ActivityVote vote = null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ActivityId", activityId)
            };

            using (var reader = DBHelper.RunProcedure(conn, "sp_SelectActivityVoteByUserId", parameters))
            {
                while (reader.Read())
                {
                    vote = ConvertToActivityVotesObject(reader);
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        vote.ActivityTotalVoteCount = reader[0].DBToInt32();
                    }
                }
            }

            return vote ?? new ActivityVote();
        }

        public ActivityVote SelectActivityVoteByWechatUserOpenId(SqlConnection conn, int activityId, string wechatUserOpenId)
        {
            ActivityVote vote = null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@WechatUserOpenId", wechatUserOpenId),
                new SqlParameter("@ActivityId", activityId)
            };

            using (var reader = DBHelper.RunProcedure(conn, "sp_SelectActivityVoteByWechatUserOpenId", parameters))
            {
                while (reader.Read())
                {
                    vote = ConvertToActivityVotesObject(reader);
                }

                if (vote == null)
                {
                    throw new ArgumentNullException("vote");
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        vote.ActivityTotalVoteCount = reader[0].DBToInt32();
                    }
                }
            }

            return vote ?? new ActivityVote();
        }

        public int SelectActivityTotalVotesCount(SqlConnection conn, int activityId)
        {
            int totalCount = 0;

            SqlParameter[] parameters =
            {
                new SqlParameter("@ActivityId", activityId)
            };

            using (var reader = DBHelper.RunProcedure(conn, "sp_SelectActivityTotalVotesCount", parameters))
            {
                while (reader.Read())
                {
                    totalCount = reader[0].DBToInt32();
                }
            }

            return totalCount;
        }

        #endregion

        #region common

        private ActivityVote ConvertToActivityVotesObject(SqlDataReader reader)
        {
            return new ActivityVote()
            {
                VoteId = reader["VoteId"].DBToInt32(),
                UserId = reader["UserId"].DBToInt32(),
                WechatUserOpenId = reader["WechatUserOpenId"].DBToString(),
                ActivityId = reader["ActivityId"].DBToInt32(),
                IsVoted = reader["IsVoted"].DBToBoolean(),
                InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime(DateTime.Now),
                LastUpdatedTimestamp = reader["LastUpdatedTimestamp"].DBToDateTime(DateTime.Now)
            };
        }

        public Activity ConvertToActivityObject(SqlDataReader reader)
        {
            Activity activity =new Activity();

            activity.ActivityType = reader["ActivityType"].DBToString();
            activity.Adress = reader["Adress"].DBToString();
            activity.ContentStyle = reader["ContentStyle"].DBToString();
            activity.ContentText = reader["ContentText"].DBToString();
            activity.CreatedTime = reader["CreatedTime"].DBToDateTime();
            activity.Description = reader["Description"].DBToString();
            activity.EndTime = reader["EndTime"].DBToDateTime();
            activity.HoldBy = reader["HoldBy"].DBToString();
            activity.Id = reader["Id"].DBToInt32();
            activity.ImageUrl = reader["ImageUrl"].DBToString();
            activity.Jingdu = reader["Jingdu"].DBToString();
            activity.Keywords = reader["Keywords"].DBToString();
            activity.LastUpdatedTime = reader["LastUpdatedTime"].DBToDateTime();
            activity.Link = reader["Link"].DBToString();
            activity.StartTime = reader["StartTime"].DBToDateTime();
            activity.State = reader["State"].DBToInt32();
            activity.Title = reader["Title"].DBToString();
            activity.UserId = reader["UserId"].DBToInt32();
            activity.ViewCount = reader["ViewCount"].DBToInt32();
            activity.Weidu = reader["Weidu"].DBToString();
            activity.LocationId = reader["LocationId"].DBToString();
            activity.UserName = reader["UserName"].DBToString();
            activity.IsFromMobile = reader["IsFromMobile"].DBToBoolean();
            return activity;
        }

        #endregion
    }
}
