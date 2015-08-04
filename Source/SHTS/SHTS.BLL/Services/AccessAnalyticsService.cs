using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.DTO;

namespace Witbird.SHTS.BLL.Services
{
    public class AccessAnalyticsService : Singleton<AccessAnalyticsService>
    {
        private AccessAnalyticsDao dao;

        protected override void Initialize()
        {
            dao=new AccessAnalyticsDao();
        }

        /// <summary>
        /// 记录用户行为
        /// </summary>
        /// <param name="access"></param>
        public void AccessTrack(AccessAnalytics access)
        {
            if (access == null)
            {
                return;
            }
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                dao.AccessTrack(conn, access);
            }
            catch (SqlException ex)
            {
                LogService.Log("记录用户行为失败", ex.StackTrace);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 分页查询会员信息
        /// </summary>
        /// <param name="startPageIndex">开始页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="queryType">查询类型</param>
        /// <returns></returns>
        public List<AccessAnalyticsWithUser> QuersAccess(DateTime formTime, DateTime toTime,
            int startRowIndex, int pageSize, int queryType, out int totalCount)
        {
            List<AccessAnalyticsWithUser> access = null;
            totalCount = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                access = dao.QuersAccess(conn, formTime,
                    toTime, startRowIndex, pageSize, queryType, out totalCount);
            }
            catch (SqlException ex)
            {
                LogService.Log("记录用户行为失败", ex.StackTrace);
            }
            finally
            {
                conn.Close();
            }
            return access;
        }
    }
}
