using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace SHTS.Finance
{
    public class FinanceManager
    {
        private BalanceDao balanceDao;
        private OrderDao orderDao;
        private RecordDao recordDao;
        private WithdrawDao withdrawDao;

        public FinanceManager()
        {
            balanceDao = new BalanceDao();
            orderDao = new OrderDao();
            recordDao = new RecordDao();
            withdrawDao = new WithdrawDao();
        }

        /// <summary>
        /// Gets all users' balances.
        /// </summary>
        /// <returns></returns>
        public List<FinanceBalance> GetAllUserBalances()
        {
            var balances = new List<FinanceBalance>();

            return balances;
        }

        /// <summary>
        /// Gets user's balance by user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public FinanceBalance GetFinanceBalance(int userId)
        {
            FinanceBalance balance = null;

            try
            {
                using (var conn = DBHelper.GetSqlConnection())
                {
                    conn.Open();
                    balance = balanceDao.SelectFinanceBalance(conn, userId);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("GetFinanceBalance", ex.ToString());
            }

            return balance;
        }

        /// <summary>
        /// Inserts a new finance balance record.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public FinanceBalance AddFinanceBalance(int userId)
        {
            FinanceBalance balance = null;

            try
            {
                using (var conn = DBHelper.GetSqlConnection())
                {
                    conn.Open();

                    balance = balanceDao.SelectFinanceBalance(conn, userId);
                    if (balance == null)
                    {
                        balance = new FinanceBalance()
                        {
                            UserId = userId,
                            AvailableBalance = 0,
                            FrozenBalance = 0,
                            InsertedTimestamp = DateTime.Now,
                            LastUpdatedTimestamp = DateTime.Now
                        };

                        balance.Id = balanceDao.InsertFinanceBalance(conn, balance);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("AddFinanceBalanceForNewUser", ex.ToString());
            }

            return balance;
        }

        /// <summary>
        /// Gets all finance records by user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<FinanceRecord> GetFinanceRecords(int userId)
        {
            var records = new List<FinanceRecord>();

            try
            {
                using (var conn = DBHelper.GetSqlConnection())
                {
                    conn.Open();
                    records = recordDao.SelectFinanceRecords(conn, userId);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("GetFinanceRecords", ex.ToString());
            }

            return records;
        }

        /// <summary>
        /// Gets all withdraw records which in New or Confirmed status.
        /// </summary>
        /// <returns></returns>
        public List<FinanceWithdrawRecord> GetFianceWithdrawRecords()
        {
            var records = new List<FinanceWithdrawRecord>();

            try
            {
                using (var conn = DBHelper.GetSqlConnection())
                {
                    conn.Open();
                    records = withdrawDao.SelectFinanceWithdrawRecordsByNewAndConfirmedStatus(conn);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("GetFianceWithdrawRecords", ex.ToString());
            }

            return records;
        }

        /// <summary>
        /// Gets all withdraw records for a specified user.
        /// </summary>
        /// <returns></returns>
        public List<FinanceWithdrawRecord> GetFianceWithdrawRecords(int userId)
        {
            var records = new List<FinanceWithdrawRecord>();

            try
            {
                using (var conn = DBHelper.GetSqlConnection())
                {
                    conn.Open();
                    records = withdrawDao.SelectFinanceWithdrawRecords(conn, userId);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("GetFianceWithdrawRecords", ex.ToString());
            }

            return records;
        }

        /// <summary>
        /// Gets finance withdraw record details by record id.
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public FinanceWithdrawRecord GetWithdrawRecordDetail(int recordId)
        {
            FinanceWithdrawRecord record = null;

            try
            {
                using (var conn = DBHelper.GetSqlConnection())
                {
                    conn.Open();
                    record = withdrawDao.SelectFinanceWithdrawRecord(conn, recordId);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("GetWithdrawRecordDetail", ex.ToString());
            }

            return record;
        }

        /// <summary>
        /// Creates a new finance withdraw record.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool CreateWithdrawRecord(FinanceWithdrawRecord record)
        {
            var result = false;

            try
            {
                using (var conn = DBHelper.GetSqlConnection())
                {
                    conn.Open();
                    result = withdrawDao.InsertFinanceWithdrawRecord(conn, record);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("CreateWithdrawRecord", ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// Updates withdraw record status.
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public OperationResult UpdateWithdrawRecordStatus(int recordId, WithdrawStatus newStatus)
        {
            var result = new OperationResult();
            var isSuccessful = false;

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(20)))
                {
                    using (var conn = DBHelper.GetSqlConnection())
                    {
                        conn.Open();
                        var record = withdrawDao.SelectFinanceWithdrawRecord(conn, recordId);

                        if (record != null)
                        {
                            lock (FinanceHelper.GetUserBalanceLockObject(record.UserId))
                            {
                                var balance = balanceDao.SelectFinanceBalance(conn, record.UserId);

                                if (balance.IsNotNull())
                                {
                                    switch (newStatus)
                                    {
                                        case WithdrawStatus.Confirmed:
                                            if (balance.AvailableBalance < record.Amount)
                                            {
                                                result.ErrorMessage = "账户可用余额不足以本次提现操作";
                                                isSuccessful = false;
                                            }
                                            else
                                            {
                                                record.WithdrawStatus = WithdrawStatus.Confirmed.ToString();
                                                record.LastUpdatedTimestamp = DateTime.Now;

                                                balance.AvailableBalance -= record.Amount;
                                                balance.FrozenBalance += record.Amount;
                                                balance.LastUpdatedTimestamp = DateTime.Now;

                                                isSuccessful = withdrawDao.UpdateFinanceWithdrawRecordStatus(conn, record);
                                                isSuccessful = isSuccessful && balanceDao.UpdateFinanceBalance(conn, balance);
                                            }

                                            break;
                                        case WithdrawStatus.Complete:
                                            if (balance.FrozenBalance < record.Amount)
                                            {
                                                result.ErrorMessage = "账户可用余额不足以本次提现操作";
                                                result.IsSuccessful = false;
                                            }
                                            else
                                            {
                                                record.WithdrawStatus = WithdrawStatus.Complete.ToString();
                                                record.LastUpdatedTimestamp = DateTime.Now;

                                                balance.FrozenBalance -= record.Amount;
                                                balance.LastUpdatedTimestamp = DateTime.Now;

                                                FinanceRecord fr = new FinanceRecord();

                                                fr.UserId = record.UserId;
                                                fr.Amount = -record.Amount;
                                                fr.Balance = balance.AvailableBalance;
                                                fr.Description = "用户申请提现完成，提现金额为 " + record.Amount + "元，已完成";
                                                fr.FinanceType = FinanceType.Withdraw.ToString();
                                                fr.InsertedTimestamp = DateTime.Now;
                                                fr.LastUpdatedTimestamp = DateTime.Now;

                                                isSuccessful = withdrawDao.UpdateFinanceWithdrawRecordStatus(conn, record);
                                                isSuccessful = isSuccessful && balanceDao.UpdateFinanceBalance(conn, balance);
                                                isSuccessful = isSuccessful && recordDao.InsertFinanceRecord(conn, fr);
                                            }
                                            break;
                                        case WithdrawStatus.Cancelled:
                                            if (record.WithdrawStatus != WithdrawStatus.New.ToString())
                                            {
                                                result.ErrorMessage = "该提现操作正在处理过程中或已处理完毕，无法取消";
                                                isSuccessful = false;
                                            }
                                            else
                                            {
                                                record.WithdrawStatus = WithdrawStatus.Cancelled.ToString();
                                                record.LastUpdatedTimestamp = DateTime.Now;

                                                //FinanceRecord fr = new FinanceRecord();
                                                //fr.UserId = record.UserId;
                                                //fr.Amount = record.Amount;
                                                //fr.Balance = balance.AvailableBalance;
                                                //fr.Description = "用户取消提现操作";
                                                //fr.FinanceType = FinanceType.Withdraw.ToString();
                                                //fr.InsertedTimestamp = DateTime.Now;
                                                //fr.LastUpdatedTimestamp = DateTime.Now;

                                                isSuccessful = withdrawDao.UpdateFinanceWithdrawRecordStatus(conn, record);
                                                //isSuccessful = isSuccessful && recordDao.InsertFinanceRecord(conn, fr);
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                        if (isSuccessful)
                        {
                            result.IsSuccessful = isSuccessful;
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("UpdateWithdrawRecordStatus", ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// Recharges user balance.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool RechargeUserBalance(FinanceOrder order)
        {
            var isSuccessful = false;

            try
            {
                ParameterChecker.Check(order);

                // 事务处理
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(20)))
                {
                    using (var conn = DBHelper.GetSqlConnection())
                    {
                        conn.Open();

                        var userBalance = balanceDao.SelectFinanceBalance(conn, order.UserId);

                        if (userBalance.IsNotNull())
                        {
                            lock (FinanceHelper.GetUserBalanceLockObject(order.UserId))
                            {
                                userBalance.AvailableBalance += order.Amount;
                                userBalance.LastUpdatedTimestamp = DateTime.Now;

                                FinanceRecord fr = new FinanceRecord();
                                fr.UserId = order.UserId;
                                fr.Amount = order.Amount;
                                fr.Balance = userBalance.AvailableBalance;
                                fr.Description = order.Detail;
                                fr.FinanceType = FinanceType.Recharge.ToString();
                                fr.InsertedTimestamp = DateTime.Now;
                                fr.LastUpdatedTimestamp = DateTime.Now;

                                isSuccessful = balanceDao.UpdateFinanceBalance(conn, userBalance);
                                isSuccessful = isSuccessful && recordDao.InsertFinanceRecord(conn, fr);
                            }
                        }

                        if (isSuccessful)
                        {
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("RechargeUserBalance", ex.ToString());
            }

            return isSuccessful;
        }
    }
}
