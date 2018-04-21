using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace SHTS.Finance
{
    internal class RecordDao
    {
        /// <summary>
        /// Inserts <c>FinanceRecord</c> entity to database.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> object.</param>
        /// <param name="record">The <c>FinanceRecord</c> entity to be inserted.</param>
        /// <returns>If successed then returns true, otherwise, returns false,</returns>
        public bool InsertFinanceRecord(SqlConnection conn, FinanceRecord record)
        {
            ParameterChecker.Check(conn);
            ParameterChecker.Check(record);

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", record.UserId),
                new SqlParameter("@FinanceType", record.FinanceType),
                new SqlParameter("@Amount", record.Amount),
                new SqlParameter("@Balance", record.Balance),
                new SqlParameter("@Description", record.Description),
                new SqlParameter("@InsertedTimestamp", record.InsertedTimestamp),
                new SqlParameter("@LastUpdatedTimestamp", record.LastUpdatedTimestamp)
            };

            return DBHelper.RunNonQueryProcedure(conn, StoredProcedures.SP_FinanceRecordInsert, parameters) > 0;
        }

        /// <summary>
        /// Selects <c>FinanceRecord</c> entities by user id.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> object.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns a <c>FinanceRecord</c> entity list.</returns>
        public List<FinanceRecord> SelectFinanceRecords(SqlConnection conn, int userId)
        {
            ParameterChecker.Check(conn);

            var records = new List<FinanceRecord>();
            var parameters = new SqlParameter[] { new SqlParameter("@UserId", userId)};

            using (var reader = DBHelper.RunProcedure(conn, StoredProcedures.SP_FinanceRecordSelectByUserId, parameters))
            {
                while (reader.Read())
                {
                    var record = new FinanceRecord()
                    {
                        Id = reader["Id"].DBToInt32(),
                        UserId = reader["UserId"].DBToInt32(),
                        FinanceType = reader["FinanceType"].DBToString(),
                        Amount = reader["Amount"].DBToDecimal(),
                        Balance = reader["Balance"].DBToDecimal(),
                        Description = reader["Description"].DBToString(),
                        InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime(DateTime.Now),
                        LastUpdatedTimestamp = reader["LastUpdatedTimestamp"].DBToDateTime(DateTime.Now)
                    };

                    records.Add(record);
                }
            }

            return records;
        }
    }
}
