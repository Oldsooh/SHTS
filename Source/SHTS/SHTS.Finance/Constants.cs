using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHTS.Finance
{
    public enum FinanceType
    {
        /// <summary>
        /// 需求鼓励金
        /// </summary>
        RechargeByDemandBonus,
        /// <summary>
        /// Withdraw
        /// </summary>
        Withdraw
    }

    public enum WithdrawStatus
    {
        /// <summary>
        /// Withdraw by user
        /// </summary>
        New,
        /// <summary>
        /// Administrator confirm
        /// </summary>
        Confirmed,
        /// <summary>
        /// Withdraw complete
        /// </summary>
        Complete,
        /// <summary>
        /// User cancels withdraw
        /// </summary>
        Cancelled
    }

    public static class StoredProcedures
    {
        public const string SP_FinanceBalanceInsert = "sp_FinanceBalanceInsert";

        public const string SP_FinanceBalanceSelectByUserId = "sp_FinanceBalanceSelectByUserId";

        public const string SP_FinanceBalanceUpdate = "sp_FinanceBalanceUpdate";

        public const string SP_FinanceRecordInsert = "sp_FinanceRecordInsert";

        public const string SP_FinanceRecordSelectByUserId = "sp_FinanceRecordSelectByUserId";

        public const string SP_FinanceWithdrawRecordInsert = "sp_FinanceWithdrawRecordInsert";

        public const string SP_FinanceWithdrawRecordSelectByNewAndConfirmedStatus = "sp_FinanceWithdrawRecordSelectByNewAndConfirmedStatus";

        public const string SP_FinanceWithdrawRecordSelectByRecordId = "sp_FinanceWithdrawRecordSelectByRecordId";

        public const string SP_FinanceWithdrawRecordUpdateStatus = "sp_FinanceWithdrawRecordUpdateStatus";

        public const string SP_FinanceWithdrawRecordSelectByUserId = "sp_FinanceWithdrawRecordSelectByUserId";
    }
}
