using System.Data.SqlClient;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL.Daos
{
    public class AccessRecordDao
    {
        public void InsertAccessRecord(SqlConnection conn, AccessRecord record, string tableName, string primaryId, string primaryValue, string columnName)
        {
            //@UserIP nvarchar(50),
            //@AccessUrl nvarchar(1000),
            //@TableName nvarchar(50),
            //@ColumnName nvarchar(50),
            //@PrimaryId nvarchar(50),
            //@PrimaryValue nvarchar(50),
            //@InsertedTimestamp datetime

            SqlParameter[] parameters =
            {
                new SqlParameter("@UserIP", record.UserIP),
                new SqlParameter("@AccessUrl", record.AccessUrl),
                new SqlParameter("@InsertedTimestamp", record.InsertedTimestamp),
                new SqlParameter("@TableName", tableName),
                new SqlParameter("@ColumnName", columnName),
                new SqlParameter("@PrimaryId", primaryId),
                new SqlParameter("@PrimaryValue", primaryValue)
            };

            DBHelper.RunNonQueryProcedure(conn, "sp_AccessRecordInsert", parameters);
        }

        public bool IsAccessRecordExisted(SqlConnection conn, AccessRecord record)
        {
            bool isExisted = false;
            //@UserIP nvarchar(50),
            //@AccessUrl nvarchar(1000)

            SqlParameter[] parameters =
            {
                new SqlParameter("@UserIP", record.UserIP),
                new SqlParameter("@AccessUrl", record.AccessUrl)
            };

            using (var reader = DBHelper.RunProcedure(conn, "sp_AccessRecordSelect", parameters))
            {
                while (reader.Read())
                {
                    isExisted = true;
                    break;
                }
            }

            return isExisted;
        }
    }
}
