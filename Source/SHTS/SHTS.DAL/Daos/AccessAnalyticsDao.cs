using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.DTO;

namespace Witbird.SHTS.DAL.Daos
{
    /// <summary>
    /// 网站访问统计
    /// </summary>
    public class AccessAnalyticsDao
    {
        #region const

        //存储过程名称
        private const string sp_PageAccessTrack = "sp_PageAccessTrack";

        //列名
        private const string column_UserId = "@UserId";
        private const string column_ReferrerUrl = "@ReferrerUrl";
        private const string column_AccessUrl = "@AccessUrl";
        private const string column_Operation = "@Operation";
        private const string column_PageTitle = "@PageTitle";
        private const string column_IP = "@IP";

        #endregion

        /// <summary>
        /// 记录。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public void AccessTrack(SqlConnection conn, AccessAnalytics access)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter(column_UserId, access.UserId),
				new SqlParameter(column_ReferrerUrl, access.ReferrerUrl),
				new SqlParameter(column_AccessUrl, access.AccessUrl),
				new SqlParameter(column_IP, access.IP),
				new SqlParameter(column_PageTitle, access.PageTitle),
				new SqlParameter(column_Operation, access.Operation)
           };
            DBHelper.CheckSqlSpParameter(parameters);
            DBHelper.RunNonQueryProcedure(conn, sp_PageAccessTrack, parameters);
        }

        /// <summary>
        /// 分页查询会员信息
        /// </summary>
        /// <param name="startPageIndex">开始页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="queryType">查询类型</param>
        /// <returns></returns>
        public List<AccessAnalyticsWithUser> QuersAccess(SqlConnection conn, DateTime formTime, DateTime toTime,
            int startRowIndex, int pageSize, int queryType, out int totalCount)
        {
            List<AccessAnalyticsWithUser> access = new List<AccessAnalyticsWithUser>();
            totalCount = 0;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(Constants.StartRowIndex, startRowIndex),
                new SqlParameter(Constants.PageSize, pageSize),
                new SqlParameter(Constants.QueryType, queryType),
                new SqlParameter("@FormTime", formTime),
                new SqlParameter("@ToTime", toTime)
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn,
                "sp_QueryAccessAnalytics", sqlParameters);
            while (reader.Read())
            {
                access.Add(ConvertToAccessAnalyticsObject(reader));
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    totalCount = reader[Constants.column_totalCount].DBToInt32();
                }
            }
            if (reader != null)
            {
                reader.Close();
            }
            return access;
        }

        #region Common

        private AccessAnalyticsWithUser ConvertToAccessAnalyticsObject(SqlDataReader reader)
        {
            AccessAnalyticsWithUser access = new AccessAnalyticsWithUser();
            access.UserId = reader["UserId"].DBToInt32();
            access.UserName = reader["UserName"].DBToString();
            access.IP = reader["IP"].DBToString();
            access.PageTitle = reader["PageTitle"].DBToString();
            access.Id = reader["Id"].DBToInt32();
            access.Operation = reader["Operation"].DBToString();
            access.ReferrerUrl = reader["ReferrerUrl"].DBToString();
            access.AccessUrl = reader["AccessUrl"].DBToString();
            access.CreateTime = reader["CreateTime"].DBToDateTime();
            return access;
        }

        #endregion
    }
}
