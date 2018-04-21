using System;
using System.Collections.Generic;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;
using WitBird.Common;

namespace Witbird.SHTS.BLL.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityService
    {
        private ActivityDao activityDao;

        public ActivityService()
        {
            activityDao = new ActivityDao();
        }

        /// <summary>
        /// 根据Id得到activity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Activity GetActivityById(int id)
        {
            Activity activity = null;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                activity = activityDao.GetActivityById(conn, id);
                if (activity != null)
                {
                    activity.ContentStyle = FilterHelper.Filter(activity.ContentStyle, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                    activity.ContentText = FilterHelper.Filter(activity.ContentText, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                    activity.Description = FilterHelper.Filter(activity.Description, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                }
            }
            catch (Exception e)
            {
                LogService.Log("GetActivityById失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return activity ?? new Activity();
        }


        /// <summary>
        /// 创建或者跟新
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CreateOrUpdateActivity(Activity activity)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = activityDao.CreateOrUpdateActivity(conn, activity);
            }
            catch (Exception e)
            {
                LogService.Log("CreateOrUpdateActivity失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="Criteria"></param>
        /// <returns></returns>
        public List<Activity> QueryActivities(QueryActivityCriteria Criteria)
        {
            List<Activity> activitys = null;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                activitys = activityDao.QueryActivities(conn, Criteria);
            }
            catch (Exception e)
            {
                LogService.Log("QueryActivities失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return activitys;
        }

        /// <summary>
        /// 更新activity 狀態
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public bool UpdateActivityStatu(Activity activity)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = activityDao.UpdateActivityStatu(conn, activity);
            }
            catch (Exception e)
            {
                LogService.Log("UpdateActivityStatu失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public bool AddOrUpdateActivityVoteRecord(ActivityVote vote)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = activityDao.InsertOrUpdateActivityVote(conn, vote);
            }
            catch (Exception e)
            {
                LogService.Log("AddOrUpdateActivityVoteRecord--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public ActivityVote SelectActivityVote(int activityId, int userId)
        {
            ActivityVote vote = null;
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                vote = activityDao.SelectActivityVoteByUserId(conn, activityId, userId);
            }
            catch(Exception e)
            {
                LogService.Log("SelectActivityVote--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return vote ?? new ActivityVote();
        }

        public ActivityVote SelectActivityVote(int activityId, string wechatUserOpenId)
        {
            ActivityVote vote = null;
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                vote = activityDao.SelectActivityVoteByWechatUserOpenId(conn, activityId, wechatUserOpenId);
            }
            catch (Exception e)
            {
                LogService.Log("SelectActivityVote--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return vote ?? new ActivityVote();
        }
    }
}
