using System;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Managers
{
    public class AccessRecordManager : Singleton<AccessRecordManager>
    {
        private AccessRecordDao dao;

        protected override void Initialize()
        {
            dao = new AccessRecordDao();
        }

        public void Record(AccessRecord record, string tableName, string primaryId, string primaryValue, string columnName)
        {
            try
            {
                using (var conn = DBHelper.GetSqlConnection())
                {
                    conn.Open();
                    if (!dao.IsAccessRecordExisted(conn, record))
                    {
                        dao.InsertAccessRecord(conn, record, tableName, primaryId, primaryValue, columnName);
                    }

                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
