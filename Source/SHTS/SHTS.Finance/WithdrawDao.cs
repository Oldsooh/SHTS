using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace SHTS.Finance
{
    internal class WithdrawDao
    {
        /// <summary>
        /// Selects all <c>FinanceWithdrawRecord</c> entities where it is in New or Complete status.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> entity.</param>
        /// <returns>Returns <c>FinanceWithdrawRecord</c> entities.</returns>
        internal List<FinanceWithdrawRecord> SelectFinanceWithdrawRecordsByNewAndConfirmedStatus(SqlConnection conn)
        {
            ParameterChecker.Check(conn);
            var records = new List<FinanceWithdrawRecord>();

            using (var reader = DBHelper.RunProcedure(conn, StoredProcedures.SP_FinanceWithdrawRecordSelectByNewAndConfirmedStatus, null))
            {
                while (reader.Read())
                {
                    var record = new FinanceWithdrawRecord()
                    {
                        Id = reader["Id"].DBToInt32(),
                        UserId = reader["UserId"].DBToInt32(),
                        UserName = reader["UserName"].DBToString(),
                        UserAvailableBalance = reader["AvailableBalance"].DBToDecimal(),
                        Amount = reader["Amount"].DBToDecimal(),
                        WithdrawStatus = reader["WithdrawStatus"].DBToString(),
                        BankInfo = reader["BankInfo"].DBToString(),
                        InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime(DateTime.Now),
                        LastUpdatedTimestamp = reader["LastUpdatedTimestamp"].DBToDateTime(DateTime.Now)
                    };

                    records.Add(record);
                }
            }

            return records;
        }

        /// <summary>
        /// Selects all <c>FinanceWithdrawRecord</c> entities where it is in New or Complete status.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> entity.</param>
        /// <returns>Returns <c>FinanceWithdrawRecord</c> entities.</returns>
        internal List<FinanceWithdrawRecord> SelectFinanceWithdrawRecords(SqlConnection conn, int userId)
        {
            ParameterChecker.Check(conn);
            var records = new List<FinanceWithdrawRecord>();
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };

            using (var reader = DBHelper.RunProcedure(conn, StoredProcedures.SP_FinanceWithdrawRecordSelectByUserId, parameters))
            {
                while (reader.Read())
                {
                    var record = new FinanceWithdrawRecord()
                    {
                        Id = reader["Id"].DBToInt32(),
                        UserId = reader["UserId"].DBToInt32(),
                        Amount = reader["Amount"].DBToDecimal(),
                        WithdrawStatus = reader["WithdrawStatus"].DBToString(),
                        BankInfo = reader["BankInfo"].DBToString(),
                        InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime(DateTime.Now),
                        LastUpdatedTimestamp = reader["LastUpdatedTimestamp"].DBToDateTime(DateTime.Now)
                    };

                    records.Add(record);
                }
            }

            return records;
        }

        /// <summary>
        /// Selects <c>FinanceWithdrawRecord</c> entity by record id.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> entity.</param>
        /// <param name="recordId">The record id.</param>
        /// <returns>Returns a <c>FinanceWithdrawRecord</c> entity.</returns>
        internal FinanceWithdrawRecord SelectFinanceWithdrawRecord(SqlConnection conn, int recordId)
        {
            ParameterChecker.Check(conn);
            
            FinanceWithdrawRecord record = null;
            var parameters = new SqlParameter[] { new SqlParameter("@RecordId", recordId)};

            using (var reader = DBHelper.RunProcedure(conn, StoredProcedures.SP_FinanceWithdrawRecordSelectByRecordId, parameters))
            {
                while(reader.Read())
                {
                    record = new FinanceWithdrawRecord()
                    {
                        Id = reader["Id"].DBToInt32(),
                        UserId = reader["UserId"].DBToInt32(),
                        Amount = reader["Amount"].DBToDecimal(),
                        WithdrawStatus = reader["WithdrawStatus"].DBToString(),
                        BankInfo = reader["BankInfo"].DBToString(),
                        InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime(DateTime.Now),
                        LastUpdatedTimestamp = reader["LastUpdatedTimestamp"].DBToDateTime(DateTime.Now)
                    };
                }
            }

            return record;
        }

        /// <summary>
        /// Inserts a <c>FinanceWithdrawRecord</c> entity to database.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> entity.</param>
        /// <param name="record">The <c>FinanceWithdrawRecord</c> to be inserted.</param>
        /// <returns>If successed then returns true, otherwise returns false.</returns>
        internal bool InsertFinanceWithdrawRecord(SqlConnection conn, FinanceWithdrawRecord record)
        {
            ParameterChecker.Check(conn);
            ParameterChecker.Check(record);

            var parameters = new SqlParameter[] 
            {
                new SqlParameter("@UserId", record.UserId),
                new SqlParameter("@Amount", record.Amount),
                new SqlParameter("@WithdrawStatus", record.WithdrawStatus),
                new SqlParameter("@BankInfo", record.BankInfo),
                new SqlParameter("@InsertedTimestamp", record.InsertedTimestamp),
                new SqlParameter("@LastUpdatedTimestamp", record.LastUpdatedTimestamp)
            };

            return DBHelper.RunNonQueryProcedure(conn, StoredProcedures.SP_FinanceWithdrawRecordInsert, parameters) > 0;
        }

        /// <summary>
        /// Updates <c>FinanceWithdrawRecord</c> entity's status.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> entity.</param>
        /// <param name="record">The <c>FinanceWithdrawRecord</c> to be updated.</param>
        /// <returns>Returns true if successed. Otherwise, returns false.</returns>
        internal bool UpdateFinanceWithdrawRecordStatus(SqlConnection conn, FinanceWithdrawRecord record)
        {
            ParameterChecker.Check(conn);
            ParameterChecker.Check(record);

            var parameters = new SqlParameter[] 
            {
                new SqlParameter("@RecordId", record.Id),
                new SqlParameter("@WithdrawStatus", record.WithdrawStatus),
                new SqlParameter("@LastUpdatedTimestamp", record.LastUpdatedTimestamp)
            };

            return DBHelper.RunNonQueryProcedure(conn, StoredProcedures.SP_FinanceWithdrawRecordUpdateStatus, parameters) > 0;
        }
    }
}
