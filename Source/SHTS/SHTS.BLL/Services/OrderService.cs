using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    public class OrderService
    {
        OrderDao orderDao = null;
        //TradeService tradeService = null;
        TradeDao tradeDao = null;
        UserDao userDao = null;

        public OrderService()
        {
            orderDao = new OrderDao();
            //tradeService = new TradeService();
            tradeDao = new TradeDao();
            userDao = new UserDao();
        }

        /// <summary>
        /// Gets order details by order id.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public TradeOrder GetOrderByOrderId(string orderId)
        {
            TradeOrder order = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                order = orderDao.GetOrderByOrderId(conn, orderId);
            }
            catch (Exception e)
            {
                LogService.Log("获取订单信息失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return order;
        }

        public bool DeleteOrderByOpenIdAndDemandIdForWeChatClient(string openId, int demandId)
        {
            bool isSuccessFul = false;
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                orderDao.DeleteOrderByOpenIdAndDemandIdForWeChatClient(conn, openId, demandId);
                isSuccessFul = true;
            }
            catch (Exception e)
            {
                LogService.Log("删除微信订单信息失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }

            return isSuccessFul;

        }

        /// <summary>
        /// Adds new order to database.
        /// </summary>
        /// <returns></returns>
        public TradeOrder AddNewOrder(string orderId, string subject, string body, decimal amount,
            int state, string username, string resourceUrl, int orderType, int resourceId)
        {
            bool result = false;
            TradeOrder order = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                orderId.CheckEmptyString("Order ID");
                subject.CheckEmptyString("Order Subject");
                body.CheckEmptyString("Order Body");

                if (state != (int)OrderState.New)
                {
                    throw new ArgumentException("Parameter Error");
                }

                order = new TradeOrder();
                order.OrderId = orderId;
                order.Amount = amount;
                order.Subject = subject;
                order.Body = body;
                order.UserName = username;
                order.CreatedTime = DateTime.Now;
                order.LastUpdatedTime = DateTime.Now;
                order.State = state;
                order.ResourceUrl = resourceUrl;
                order.OrderType = orderType;
                order.ResourceId = resourceId;

                conn.Open();
                result = orderDao.AddNewOrder(conn, order);
            }
            catch (Exception e)
            {
                LogService.Log("添加订单信息失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }

            return order;
        }

        /// <summary>
        /// Updates order state
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        public bool UpdateOrderState(string orderId, int newState)
        {
            bool result = false;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    conn.Open();
                    TradeOrder order = orderDao.GetOrderByOrderId(conn, orderId);
                    
                    if (order != null)
                    {
                        #region 处理中介交易在线支付结果

                        if (order.OrderType.Value == (int)OrderType.Trade && newState == (int)OrderState.Succeed)
                        {
                            Trade trade = tradeDao.SelectTradeByTradeId(order.ResourceId.Value, conn);
                            
                            if (tradeDao.UpdateTradeOrderIdAndBuyerPaid(trade.TradeId, orderId, true, conn))
                            {
                                string subject = "买家" + order.UserName + "在线支付中介交易款项成功";
                                string body = "买家已在线支付中介交易款项成功，卖家可以发货";

                                TradeHistory history = new TradeHistory
                                {
                                    HistorySubject = subject,
                                    HistoryBody = body,
                                    TradeId = trade.TradeId,
                                    UserId = trade.BuyerId,
                                    UserName = order.UserName,
                                    IsAdminUpdate = false,
                                    TradeState = trade.State,
                                    CreatedTime = DateTime.Now
                                };
                                
                                result = tradeDao.ReplyTradeWithOperation(history, conn);
                            }
                        }
                        #endregion

                        #region 处理升级为VIP会员在线支付结果
                        else if (order.OrderType.Value == (int)OrderType.ToVip && newState == (int)OrderState.Succeed)
                        {
                            UserVip vipInfo = userDao.SelectUserVipInfoByUserId(conn, order.ResourceId.Value);

                            vipInfo.State = (int)VipState.VIP;
                            vipInfo.StartTime = DateTime.Now;
                            vipInfo.EndTime = DateTime.Now.AddYears(vipInfo.Duration.Value);
                            vipInfo.LastUpdatedTime = DateTime.Now;

                            result = userDao.UpdateUserVipState(conn, order.ResourceId.Value, (int)VipState.VIP);
                            if (result)
                            {
                                result = userDao.UpdateUserVipInfo(conn, vipInfo);
                            }
                        }
                        //else if (order.OrderType.Value == (int)OrderType.WeChatDemand && newState == (int)OrderState.Succeed)
                        //{
                            
                        //}
                        else
                        {
                            result = true;
                        }
                        #endregion

                        // 更新订单信息
                        if (result)
                        {
                            result = orderDao.UpdateOrderState(conn, orderId, newState, DateTime.Now);
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                LogService.Log("更新订单状态失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        private object OrderNumberBuilderLock = new object();

        /// <summary>
        /// Generates a new order number.
        /// </summary>
        /// <returns></returns>
        public string GenerateNewOrderNumber()
        {
            StringBuilder orderNumberBuilder = new StringBuilder();

            lock (OrderNumberBuilderLock)
            {
                int sequenceNumber = 1;
                DateTime resetDate = DateTime.Now;

                if (Caching.Get("OrderSequenceNumer") != null)
                {
                    sequenceNumber = Convert.ToInt32(Caching.Get("OrderSequenceNumer"));
                }

                if (Caching.Get("OrderResetDate") != null)
                {
                    resetDate = Convert.ToDateTime(Caching.Get("OrderResetDate"));
                }

                //int oneDay = 24 * 60;
                // Reset
                if (resetDate.Date != DateTime.Now.Date)
                {
                    sequenceNumber = 1;
                    resetDate = DateTime.Now;

                    //Caching.Set("OrderReseetDate", resetDate, oneDay);
                    Caching.Set("OrderResetDate", resetDate);
                }

                sequenceNumber += 1;
                //Caching.Set("OrderSequenceNumer", sequenceNumber, oneDay);
                Caching.Set("OrderSequenceNumer", sequenceNumber);

                orderNumberBuilder.Append(resetDate.ToString("yyyyMMddhhmmss"));
                orderNumberBuilder.Append(sequenceNumber.ToString().PadLeft(4, '0'));
            }

            return orderNumberBuilder.ToString();
        }

        /// <summary>
        /// delete order id.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool DeleteOrderById(string orderId)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                result = orderDao.DeleteOrderById(conn, orderId);
            }
            catch (Exception e)
            {
                LogService.Log("删除订单" + orderId + "失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Converts order's state from db store format to display string on UI.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string ConvertOrderStateToDisplayValue(int state)
        {
            string result = string.Empty;

            switch (state)
            {
                case (int)OrderState.Failed:
                    result = "支付失败";
                    break;
                case (int)OrderState.Invalid:
                    result = "无效订单";
                    break;
                case (int)OrderState.New:
                    result = "等待付款";
                    break;
                case (int)OrderState.Succeed:
                    result = "支付成功";
                    break;
                default:
                    result = "未知类型";
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获取用户的购买记录
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<TradeOrder> GetWeChatUserPaidDemands(string openId, int pageSize, int pageIndex, out int totalCount)
        {
            List<TradeOrder> result = new List<TradeOrder>();

            totalCount = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                result = orderDao.SelectUserPaidDemands(conn, openId, pageSize, pageIndex, out tempCount);
                totalCount = tempCount;
                
            }
            catch (Exception e)
            {
                LogService.Log("查询我的购买记录", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }
    }
}
