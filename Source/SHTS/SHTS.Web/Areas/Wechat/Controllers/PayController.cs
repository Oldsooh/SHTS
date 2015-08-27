using System;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;
using WitBird.Com.Pay;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class PayController : WechatBaseController
    {
        //
        // GET: /Pay/
        OrderService orderService;

        public PayController()
        {
            orderService = new OrderService();
        }

        /// <summary>
        /// 提交订单信息到订单页面
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Index(string orderId)
        {
            OrderModel model = new OrderModel();
            //model.ReturnUrl = returnUrl;

            try
            {
                orderId.CheckEmptyString("Order ID");
                model.Order = orderService.GetOrderByOrderId(orderId);
            }
            catch (Exception e)
            {
                LogService.Log("生成订单页面数据失败", e.ToString());
            }

            return View(model);
        }


        #region 私有方法

        /// <summary>
        /// Updates order state if online pay complete
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool UpdateOrder(PayResult result, out string returnUrl)
        {
            bool success = false;
            returnUrl = string.Empty;

            if (result != null)
            {
                TradeOrder order = orderService.GetOrderByOrderId(result.OrderId);
                if (order != null && result.Amount == order.Amount)
                {
                    int newState = -1;

                    switch (result.TradeStatus)
                    {
                        case PayStatus.Failed:
                            newState = (int)OrderState.Failed;
                            break;
                        case PayStatus.Invalid:
                            newState = (int)OrderState.Invalid;
                            break;
                        case PayStatus.Success:
                            newState = (int)OrderState.Succeed;
                            break;
                        case PayStatus.UnKnow:
                        default:
                            newState = (int)OrderState.Invalid;
                            break;
                    }

                    success = true;
                    if (order.State != newState)
                    {
                        success = orderService.UpdateOrderState(order.OrderId, newState);
                    }

                    if (success)
                    {
                        returnUrl = order.ResourceUrl;
                    }
                }
            }

            return success;
        }

        #endregion 私有方法
    }
}
