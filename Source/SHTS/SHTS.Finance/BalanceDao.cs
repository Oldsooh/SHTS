using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;
using Witbird.SHTS.DAL;
using System.Data.SqlClient;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;

namespace SHTS.Finance
{
    internal class BalanceDao
    {
        /// <summary>
        /// Selects <c>FinanceBalance</c> entity by specified user id.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> object.</param>
        /// <param name="userId">The specified user id.</param>
        /// <returns>Returns a <c>FinanceBalance</c> entity </returns>
        internal FinanceBalance SelectFinanceBalance(SqlConnection conn, int userId)
        {
            FinanceBalance balance = null;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };

            using (var reader = DBHelper.RunProcedure(conn, StoredProcedures.SP_FinanceBalanceSelectByUserId, parameters))
            {
                while (reader.Read())
                {
                    balance = new FinanceBalance()
                    {
                        Id = reader["Id"].DBToInt32(),
                        UserId = reader["UserId"].DBToInt32(),
                        AvailableBalance = reader["AvailableBalance"].DBToDecimal(),
                        FrozenBalance = reader["FrozenBalance"].DBToDecimal(),
                        InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime(DateTime.Now),
                        LastUpdatedTimestamp = reader["LastUpdatedTimestamp"].DBToDateTime(DateTime.Now)
                    };
                }
            }

            return balance;
        }

        /// <summary>
        /// Inserts <c>FinanceBalance</c> entity to database.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> object.</param>
        /// <param name="balance">The <c>FinanceBalance</c> entity to be inserted.</param>
        /// <returns>Returns the balanceId. If equals 0 then failed. Otherwise successed.</returns>
        internal int InsertFinanceBalance(SqlConnection conn, FinanceBalance balance)
        {
            ParameterChecker.Check(balance);
            int balanceId = 0;
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", balance.UserId),
                new SqlParameter("@AvailableBalance", balance.AvailableBalance),
                new SqlParameter("@FrozenBalance", balance.FrozenBalance),
                new SqlParameter("@InsertedTimestamp", balance.InsertedTimestamp),
                new SqlParameter("@LastUpdatedTimestamp", balance.LastUpdatedTimestamp)
            };

            using (var reader = DBHelper.RunProcedure(conn, StoredProcedures.SP_FinanceBalanceInsert, parameters))
            {
                while (reader.Read())
                {
                    balanceId = reader[0].DBToInt32();
                }
            }

            return balanceId;
        }

        /// <summary>
        /// Updates <c>FinanceBalance</c> entity to database.
        /// </summary>
        /// <param name="conn">A opened <c>SqlConnection</c> object.</param>
        /// <param name="balance">The <c>FinanceBalance</c> entity to be updated.</param>
        /// <returns>Returns true then update successed, otherwise, failed.</returns>
        internal bool UpdateFinanceBalance(SqlConnection conn, FinanceBalance balance)
        {
            ParameterChecker.Check(balance);

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@BalanceId", balance.Id),
                new SqlParameter("@AvailableBalance", balance.AvailableBalance),
                new SqlParameter("@FrozenBalance", balance.FrozenBalance),
                new SqlParameter("@LastUpdatedTimestamp", balance.LastUpdatedTimestamp)
            };
            
            return DBHelper.RunNonQueryProcedure(conn, StoredProcedures.SP_FinanceBalanceUpdate, parameters) > 0;
        }
    }
}
