using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    public class TradeService
    {
        TradeDao tradeDao;
        PublicConfigService publicConfigService;

        public TradeService()
        {
            tradeDao = new TradeDao();
            publicConfigService = new PublicConfigService();
        }

        /// <summary>
        /// Adds new trade record.
        /// </summary>
        /// <param name="tradeEntity"></param>
        /// <returns></returns>
        public bool AddNewTradeRecord(Trade tradeEntity)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = tradeDao.AddTradeRecord(conn, tradeEntity);
            }
            catch (Exception e)
            {
                LogService.Log("中介申请失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// Gets trade config by config name.
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public PublicConfig GetTradeConfig(string configName)
        {
            return publicConfigService.GetConfigValue(configName);
        }

        /// <summary>
        /// Gets trade config by config id.
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public PublicConfig GetTradeConfig(int configId)
        {
            return publicConfigService.GetConfigValue(configId);
        }

        /// <summary>
        /// Gets trade list.
        /// </summary>
        /// <param name="pageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Trade> GetTradeList(int pageCount, int pageIndex, int tradeState, out int count)
        {
            List<Trade> result = new List<Trade>();

            count = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                result = tradeDao.SelectTradeList(pageCount, pageIndex, tradeState, out tempCount, conn);
                count = tempCount;
            }
            catch (Exception e)
            {
                LogService.Log("根据用户ID查询中介列表失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Gets trade list by user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Trade> GetTradeListByUserId(int userId, int pageCount, int pageIndex, out int count)
        {
            List<Trade> result = new List<Trade>();

            count = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                result = tradeDao.SelectTradeListByUserId(userId, pageCount, pageIndex, out tempCount, conn);
                count = tempCount;
            }
            catch (Exception e)
            {
                LogService.Log("根据用户ID查询中介列表失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Gets the trade detail includes buyer and seller information.
        /// </summary>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        public Trade GetTradeByTradeId(int tradeId)
        {
            Trade result = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = tradeDao.SelectTradeByTradeId(tradeId, conn);

            }
            catch (Exception e)
            {
                LogService.Log("查询中介详情失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Replies trade with operation, adds a new record to table TradeHistory.
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public bool ReplyTradeWithOperation(string historySubject, string historyBody, int tradeId,
            int userId, string username, bool isAdminUpdate, TradeState tradeState, DateTime createdTime)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                TradeHistory history = new TradeHistory
                {
                    HistorySubject = historySubject,
                    HistoryBody = historyBody,
                    TradeId = tradeId,
                    UserId = userId,
                    UserName = username,
                    IsAdminUpdate = isAdminUpdate,
                    TradeState = (int)tradeState,
                    CreatedTime = createdTime
                };

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    conn.Open();
                    result = tradeDao.ReplyTradeWithOperation(history, conn) &&
                        tradeDao.UpdateTradeState(tradeId, (int)tradeState, conn);

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                LogService.Log("回复中介失败", e.ToString());
                result = false;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Update trade state.
        /// </summary>
        /// <param name="tradeId"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        public bool UpdateTradeState(int tradeId, int newState)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                conn.Open();
                result = tradeDao.UpdateTradeState(tradeId, newState, conn);
            }
            catch (Exception e)
            {
                LogService.Log("更新交易状态失败", e.ToString());
                result = false;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public bool UpdateTradeOrderId(int tradeId, string orderId, bool isBuyerPaid)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                conn.Open();
                result = tradeDao.UpdateTradeOrderIdAndBuyerPaid(tradeId, orderId, isBuyerPaid, conn);
            }
            catch (Exception e)
            {
                LogService.Log("更新交易订单号失败", e.ToString());
                result = false;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// Translates trade state from a numeric to UI display value.
        /// </summary>
        /// <param name="tradeState"></param>
        /// <returns></returns>
        public static string ConvertStateToDisplayMode(int tradeState)
        {
            string result = string.Empty;

            switch (tradeState)
            {
                case (int)TradeState.New:
                    result = "中介申请";
                    break;
                case (int)TradeState.Completed:
                    result = "交易完成";
                    break;
                case (int)TradeState.Finished:
                    result = "交易终止";
                    break;
                case (int)TradeState.InProgress:
                    result = "交易进行";
                    break;
                case (int)TradeState.Invalid:
                    result = "违规交易";
                    break;
                default:
                    result = "未知交易类型";
                    break;
            }

            return result;
        }

        public void CheckReplyTradeParameters(string operation, string content)
        {
            if (operation.Equals("update", StringComparison.CurrentCultureIgnoreCase) &&
                operation.Equals("reviewed", StringComparison.CurrentCultureIgnoreCase) &&
                operation.Equals("delete", StringComparison.CurrentCultureIgnoreCase) &&
                operation.Equals("completed", StringComparison.CurrentCultureIgnoreCase) &&
                operation.Equals("finished", StringComparison.CurrentCultureIgnoreCase) &&
                operation.Equals("invalid", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("交易操作动作不正确，请重新选择");
            }
            else if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("回复内容不能为空");
            }
            else
            {
                // Nothing to do.
            }
        }

        public void CheckTradeState(int originalState, int newState)
        {
            if (originalState == (int)TradeState.Completed ||
                originalState == (int)TradeState.Finished ||
                originalState == (int)TradeState.Invalid)
            {
                throw new ArgumentException("交易状态已不允许发生更改，请重新获取最新交易信息");
            }

            if (newState == (int)TradeState.New ||
                newState < originalState)
            {
                throw new ArgumentException("当前交易操作动作不正确，请重新获取最新交易信息后尝试");
            }
        }

        public int ConvertToTradeStateFromOperation(string operation, int orginalState)
        {
            int tradeState = orginalState;
            switch (operation.ToLower())
            {
                case "reviewed":
                    tradeState = (int)TradeState.InProgress;
                    break;
                case "finished":
                case "delete":
                    tradeState = (int)TradeState.Finished;
                    break;
                case "completed":
                    tradeState = (int)TradeState.Completed;
                    break;
                case "invalid":
                    tradeState = (int)TradeState.Invalid;
                    break;
                case "update":
                case "paid":
                    break;
                default:
                    throw new ArgumentException("操作动作错误");
            }

            return tradeState;
        }
    }
}
