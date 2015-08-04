using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL.Daos
{
    public class TradeDao
    {
        #region SP const name

        private const string SP_AddNewTrade = "sp_AddNewTrade";
        private const string SP_SelectTradeListByUserId = "sp_SelectTradeListByUserId";
        private const string SP_SelectTradeList = "sp_SelectTradeList";
        private const string SP_SelectTradeByTradeId = "sp_SelectTradeByTradeId";

        #endregion SP const name

        /// <summary>
        /// Adds new trade record.
        /// </summary>
        /// <param name="tradeEntiy"></param>
        /// <returns></returns>
        public bool AddTradeRecord(SqlConnection conn, Trade tradeEntiy)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter("@TradeId", SqlDbType.Int, 4),
				new SqlParameter("@UserId", tradeEntiy.UserId),
				new SqlParameter("@UserQQ", tradeEntiy.UserQQ),
				new SqlParameter("@UserCellPhone", tradeEntiy.UserCellPhone),
				new SqlParameter("@UserEmail", tradeEntiy.UserEmail),
				new SqlParameter("@UserBankInfo", tradeEntiy.UserBankInfo),
				new SqlParameter("@UserAddress", tradeEntiy.UserAddress),
				new SqlParameter("@SellerId", tradeEntiy.SellerId),
				new SqlParameter("@BuyerId", tradeEntiy.BuyerId),
				new SqlParameter("@TradeAmount", tradeEntiy.TradeAmount),
				new SqlParameter("@TradeSubject", tradeEntiy.TradeSubject),
				new SqlParameter("@TradeBody", tradeEntiy.TradeBody),
				new SqlParameter("@Payer", tradeEntiy.Payer),
				new SqlParameter("@PayCommission", tradeEntiy.PayCommission),
				new SqlParameter("@PayCommissionPercent", tradeEntiy.PayCommissionPercent),
				new SqlParameter("@CreatedTime", tradeEntiy.CreatedTime),
                new SqlParameter("@LastUpdatedTime", tradeEntiy.LastUpdatedTime),
                new SqlParameter("@State", tradeEntiy.State),
                new SqlParameter("@SellerGet", tradeEntiy.SellerGet),
                new SqlParameter("@BuyerPay", tradeEntiy.BuyerPay),
                new SqlParameter("@ViewCount", tradeEntiy.ViewCount),
                new SqlParameter("@ResourceUrl", tradeEntiy.ResourceUrl),
                new SqlParameter("@IsBuyerPaid", tradeEntiy.IsBuyerPaid),
                new SqlParameter("@TradeOrderId", tradeEntiy.TradeOrderId),
           };

            parameters[0].Direction = ParameterDirection.Output;
            DBHelper.CheckSqlSpParameter(parameters);

            return DBHelper.RunNonQueryProcedure(conn, SP_AddNewTrade, parameters) > 0;
        }

        /// <summary>
        /// Gets all trades.
        /// </summary>
        public List<Trade> SelectTradeList(int pageCount, int pageIndex, int tradeState, out int count, SqlConnection conn)
        {
            List<Trade> result = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageCount", pageCount),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@State", tradeState)
            };

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, SP_SelectTradeList, sqlParameters);
            count = Int32.Parse(dts["0"].Rows[0][0].ToString());
            result = DBHelper.DataTableToList<Trade>(dts["1"]);

            return result ?? new List<Trade>();
        }

        /// <summary>
        /// Gets trade list by user id.
        /// </summary>
        public List<Trade> SelectTradeListByUserId(int userId, int pageCount, int pageIndex, out int count, SqlConnection conn)
        {
            List<Trade> result = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@PageCount", pageCount),
                new SqlParameter("@PageIndex", pageIndex)
            };
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, SP_SelectTradeListByUserId, sqlParameters);
            count = Int32.Parse(dts["0"].Rows[0][0].ToString());
            result = DBHelper.DataTableToList<Trade>(dts["1"]);

            return result ?? new List<Trade>();
        }

        /// <summary>
        /// Gets trade list by trade id.
        /// </summary>
        public Trade SelectTradeByTradeId(int tradeId, SqlConnection conn)
        {
            Trade result = new Trade();
            result.Seller = new User();
            result.Buyer = new User();

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@TradeId", tradeId)
            };

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, SP_SelectTradeByTradeId, sqlParameters);
            List<Trade> tempTrades = DBHelper.DataTableToList<Trade>(dts["0"]);
            List<User> tempSellers = DBHelper.DataTableToList<User>(dts["1"]);
            List<User> tempBuyers = DBHelper.DataTableToList<User>(dts["2"]);
            List<UserBankInfo> tempBanks = DBHelper.DataTableToList<UserBankInfo>(dts["3"]);

            if (tempTrades != null && tempTrades.Count == 1)
            {
                result = tempTrades[0];

                if (result != null)
                {
                    if (tempSellers != null && tempSellers.Count == 1)
                    {
                        result.Seller = tempSellers[0] ?? new User();
                    }

                    if (tempBuyers != null && tempBuyers.Count == 1)
                    {
                        result.Buyer = tempBuyers[0] ?? new User();
                    }

                    result.BankInfos = tempBanks ?? new List<UserBankInfo>();
                    result.Histories = SelectTradeHistories(tradeId, conn)?? new List<TradeHistory>();
                }
            }

            return result;
        }

        private List<TradeHistory> SelectTradeHistories(int tradeId, SqlConnection conn)
        {
            List<TradeHistory> result = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@TradeId", tradeId)
            };

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, "sp_SelectTradeHistory", sqlParameters);
            result = DBHelper.DataTableToList<TradeHistory>(dts["0"]);

            return result;
        }

        /// <summary>
        /// Replies trade with operation, adds a new record to table TradeHistory.
        /// </summary>
        /// <param name="history"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public bool ReplyTradeWithOperation(TradeHistory history, SqlConnection conn)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@HistorySubject", history.HistorySubject),
                new SqlParameter("@HistoryBody", history.HistoryBody),
                new SqlParameter("@TradeId", history.TradeId),
                new SqlParameter("@UserId", history.UserId),
                new SqlParameter("@UserName", history.UserName),
                new SqlParameter("@IsAdminUpdate", history.IsAdminUpdate),
                new SqlParameter("@TradeState", history.TradeState),
                new SqlParameter("@CreatedTime", history.CreatedTime)
            };

            return DBHelper.RunNonQueryProcedure(conn, "sp_ReplyTradeWithOperation", sqlParameters) > 0;
        }

        /// <summary>
        /// Updates trade state by trade id.
        /// </summary>
        /// <param name="tradeId"></param>
        /// <param name="tradeState"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public bool UpdateTradeState(int tradeId, int tradeState, SqlConnection conn)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@TradeId", tradeId),
                new SqlParameter("@TradeState", tradeState),
                new SqlParameter("@LastUpdatedTime", DateTime.Now)
            };

            return DBHelper.RunNonQueryProcedure(conn, "sp_UpdateTradeState", sqlParameters) > 0;
        }

        public bool UpdateTradeOrderIdAndBuyerPaid(int tradeId, string orderId, bool isBuyerPaid, SqlConnection conn)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@TradeId", tradeId),
                new SqlParameter("@IsBuyerPaid", isBuyerPaid),
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@LastUpdatedTime", DateTime.Now)
            };

            return DBHelper.RunNonQueryProcedure(conn, "sp_UpdateTradeOrderId", sqlParameters) > 0;
        }
    }
}
