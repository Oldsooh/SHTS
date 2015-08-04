using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;

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
            }
            catch (Exception e)
            {
                LogService.Log("GetActivityById失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return activity;
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

    }
}
