using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;
using Witbird.SHTS.Common;
using System.Data;

namespace Witbird.SHTS.DAL.Daos
{
    public class OrderDao
    {
        /// <summary>
        /// Gets order details by order id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public TradeOrder GetOrderByOrderId(SqlConnection conn, string orderId)
        {
            TradeOrder order = null;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };

            SqlDataReader reader = DBHelper.RunProcedure(conn, "sp_SelectOrderByOrderId", sqlParameters);

            while (reader.Read())
            {
                order = new TradeOrder()
                {
                    OrderId = orderId,
                    Amount = reader["Amount"].DBToDecimal(),
                    Body = reader["Body"].DBToString(),
                    CreatedTime = reader["CreatedTime"].DBToDateTime().Value,
                    LastUpdatedTime = reader["LastUpdatedTime"].DBToDateTime().Value,
                    State = reader["State"].DBToInt32(),
                    Subject = reader["Subject"].DBToString(),
                    UserName = reader["UserName"].DBToString(),
                    ResourceUrl = reader["ResourceUrl"].DBToString(),
                    OrderType = reader["OrderType"].DBToInt32(),
                    ResourceId = reader["ResourceId"].DBToInt32()
                };
            }

            if (reader != null)
            {
                reader.Close();
            }

            return order;
        }

        public void DeleteOrderByOpenIdAndDemandIdForWeChatClient(SqlConnection conn, string openId, int demandId)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserName", openId),
                new SqlParameter("@ResourceId", demandId)
            };

            DBHelper.RunNonQueryProcedure(conn, "sp_DeleteOrderByOpenIdAndDemandIdForWeChatClient", sqlParameters);
        }

        /// <summary>
        /// Adds new order to database.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool AddNewOrder(SqlConnection conn, TradeOrder order)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", order.OrderId),
                new SqlParameter("@Amount", order.Amount),
                new SqlParameter("@Subject", order.Subject),
                new SqlParameter("@Body", order.Body),
                new SqlParameter("@UserName", order.UserName),
                new SqlParameter("@CreatedTime", order.CreatedTime),
                new SqlParameter("@LastUpdatedTime", order.LastUpdatedTime),
                new SqlParameter("@State", order.State),
                new SqlParameter("@ResourceUrl", order.ResourceUrl),
                new SqlParameter("@OrderType", order.OrderType),
                new SqlParameter("@ResourceId", order.ResourceId)
            };

            return DBHelper.RunNonQueryProcedure(conn, "sp_AddNewOrder", sqlParameters) > 0;
        }

        /// <summary>
        /// Checks the new order number is exist in database or not.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool IsOrderNumberExist(SqlConnection conn, string orderId)
        {
            string result = string.Empty;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };

            SqlDataReader reader = DBHelper.RunProcedure(conn, "sp_SelectExistOrderId", sqlParameters);

            while (reader.Read())
            {
                result = reader["OrderId"].DBToString();
            }

            return !string.IsNullOrEmpty(result);
        }

        /// <summary>
        /// Updates order state.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="orderId"></param>
        /// <param name="newState"></param>
        /// <param name="updatedTime"></param>
        /// <returns></returns>
        public bool UpdateOrderState(SqlConnection conn, string orderId, int newState, DateTime updatedTime)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@State", newState),
                new SqlParameter("@LastUpdatedTime", updatedTime)
            };

            return DBHelper.RunNonQueryProcedure(conn, "sp_UpdateOrderState", sqlParameters) > 0;
        }

        /// <summary>
        /// Delete order by id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool DeleteOrderById(SqlConnection conn, string orderId)
        {
            SqlCommand cmd = new SqlCommand("delete from TradeOrder where OrderId = " + orderId, conn);
            return cmd.ExecuteNonQuery() > 0;
        }

        public List<TradeOrder> SelectUserPaidDemands(SqlConnection conn, string openId, int pageSize, int pageIndex, out int totalCount)
        {
            List<TradeOrder> paidDemands = null;
            totalCount = 0;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserName", openId),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@PageIndex", pageIndex)
            };

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, "sp_SelectUserPaidDemands", sqlParameters);
            totalCount = Int32.Parse(dts["0"].Rows[0][0].ToString());
            paidDemands = DBHelper.DataTableToList<TradeOrder>(dts["1"]);

            return paidDemands;
        }
    }
}
