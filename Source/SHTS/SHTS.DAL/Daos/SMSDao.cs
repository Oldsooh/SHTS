using System.Data.SqlClient;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL
{
    /// <summary>
    /// 短信数据库操作
    /// </summary>
    public class SMSDao
    {
        #region const

        //存储过程名称
        private const string sp_AddSMSRecord = "sp_AddSMSRecord";

        //列名
        private const string column_UserName = "@UserId";

        #endregion

        /// <summary>
        /// 注册用户。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddSMSRecord(SqlConnection conn, ShortMessage message)
        {
            SqlParameter[] parameters = 
            {
				new SqlParameter("@Provider", message.Provider),
				new SqlParameter("@Cellphone", message.Cellphone),
				new SqlParameter("@State", message.State)
           };
            DBHelper.CheckSqlSpParameter(parameters);
            return DBHelper.RunNonQueryProcedure(conn, sp_AddSMSRecord, parameters) > 0;
        }
    }
}
