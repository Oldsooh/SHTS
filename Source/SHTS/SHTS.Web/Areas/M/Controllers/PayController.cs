using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Controllers;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Public;
using WitBird.Com.Pay;

namespace Witbird.SHTS.Web.Areas.M.Controllers
{
    public class PayController : BaseController
    {
        //
        // GET: /Pay/
        IPayService payService = null;
        OrderService orderService;
        PublicConfigService publicConfigService = null;

        public PayController()
        {
            orderService = new OrderService();
            publicConfigService = new PublicConfigService();
        }

        /// <summary>
        /// 提交订单信息到订单页面
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateOrder(string orderId, string returnUrl)
        {
            OrderModel model = new OrderModel();
            model.ReturnUrl = returnUrl;

            try
            {
                orderId.CheckEmptyString("Order ID");
                model.Order = orderService.GetOrderByOrderId(orderId);

                if (model.Order.State == (int)Witbird.SHTS.Model.OrderState.New ||
                    model.Order.State == (int)Witbird.SHTS.Model.OrderState.Failed)
                {
                    model.OfflineBankInfos = publicConfigService.GetConfigValues(Constant.TradeBankInfoConfig);
                }
            }
            catch (Exception e)
            {
                LogService.Log("生成订单页面数据失败", e.ToString());
            }

            return View("Index", model);
        }

        /// <summary>
        /// 提交支付请求到支付平台
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PayOnline(string orderId, string payment)
        {
            string responseHtml = "";

            try
            {
                // 取消其他在线支付方式,只支持支付宝,强制设置
                payment = PaymentService.ALIPAYSERVICE;

                //客户端IP地址
                string clientIPAddr = GetUserIp();

                payService = BuildPayService(payment);

                if (payService != null)
                {
                    TradeOrder order = orderService.GetOrderByOrderId(orderId);

                    if (order != null && !string.IsNullOrEmpty(order.OrderId))
                    {
                        PayRequestCriteria criteria = new PayRequestCriteria();
                        criteria.OrderId = order.OrderId;
                        criteria.Amount = order.Amount.ToString();
                        criteria.Subject = order.Subject;
                        criteria.Body = order.Body;
                        criteria.ClientIP = clientIPAddr;
                        //criteria.AddCustomParameter("", "");

                        responseHtml = payService.BuildRequest(criteria);
                    }
                    else
                    {
                        responseHtml = "订单：" + orderId + "不存在";
                    }
                }
                else
                {
                    responseHtml = "The pay service " + payment + " does not support.";
                }
            }
            catch (Exception e)
            {
                LogService.Log("PayOnline Failed", e.ToString());
            }

            return Content(responseHtml);
        }

        #region 支付结果处理方法

        /// <summary>
        /// 处理支付宝同步通知结果， 必须为HttpGet方式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AlipayResult()
        {
            string returnUrl = string.Empty;
            string orderId = HandlePayResultAndNotify(PaymentService.ALIPAYSERVICE, false, out returnUrl);
            return Content(string.Format(Constant.PostPayInfoFormatForMobile, orderId, returnUrl));
        }

        /// <summary>
        /// 处理支付宝异步通知结果，必须为HttpPost方式
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AlipayNotify()
        {
            string returnUrl = string.Empty;
            string code = HandlePayResultAndNotify(PaymentService.ALIPAYSERVICE, true, out returnUrl);
            return Content(code);
        }

        ///// <summary>
        ///// 处理网银在线同步通知结果，必须为HttpPost方式
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult ChinabankResult()
        //{
        //    string returnUrl = string.Empty;
        //    string orderId = HandlePayResultAndNotify(PaymentService.CHINABANKSERVICE, false, out returnUrl);
        //    return Content(string.Format(Constant.PostPayInfoFormat, orderId, returnUrl));
        //}

        ///// <summary>
        ///// 处理网银在线异步通知结果，必须为HttpPost方式
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult ChinabankNotify()
        //{
        //    string returnUrl = string.Empty;
        //    string code = HandlePayResultAndNotify(PaymentService.CHINABANKSERVICE, true, out returnUrl);
        //    return Content(code);
        //}

        ///// <summary>
        ///// 处理财付通同步通知结果，必须为HttpGet方式
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult TenpayResult()
        //{
        //    string returnUrl = string.Empty;
        //    string orderId = HandlePayResultAndNotify(PaymentService.TENPAYSERVICE, false, out returnUrl);
        //    return Content(string.Format(Constant.PostPayInfoFormat, orderId, returnUrl));
        //}

        ///// <summary>
        ///// 处理财付通异步通知结果，必须为HttpGet方式
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult TenpayNotify()
        //{
        //    string returnUrl = string.Empty;
        //    string code = HandlePayResultAndNotify(PaymentService.TENPAYSERVICE, true, out returnUrl);
        //    return Content(code);
        //}

        #endregion 支付结果处理方法

        #region 私有方法

        /// <summary>
        /// Gets user client ip address.
        /// </summary>
        private string GetUserIp()
        {
            string realRemoteIP = "";
            if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                realRemoteIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0];
            }
            if (string.IsNullOrEmpty(realRemoteIP))
            {
                realRemoteIP = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(realRemoteIP))
            {
                realRemoteIP = Request.UserHostAddress;
            }
            return realRemoteIP;
        }
        
        /// <summary>
        /// Handles online payment service async or sync notify.
        /// </summary>
        /// <param name="payServiceName"></param>
        /// <param name="isNotify"></param>
        /// <returns></returns>
        private string HandlePayResultAndNotify(string payServiceName, bool isNotify, out string returnUrl)
        {
            string responseCodeOrOrderId = string.Empty;
            returnUrl = string.Empty;

            try
            {
                payService = BuildPayService(payServiceName);
                payService.CheckNullObject("Online Payment Service");
                
                NameValueCollection reqResult = BuildRequestResult(payService, isNotify);
                responseCodeOrOrderId = payService.ResponseCodeFailed;

                if (payService.Verify(reqResult))
                {
                    PayResult result = payService.ParseResult(reqResult);
                    if (UpdateOrder(result, out returnUrl))
                    {
                        responseCodeOrOrderId = payService.ResponseCodeSucceed;
                    }

                    if (!isNotify)
                    {
                        responseCodeOrOrderId = result.OrderId;
                    }
                }
                else
                {
                    LogService.Log(payService.CurrentPayment + " verify failed.", reqResult.ToString());
                }
            }
            catch (Exception e)
            {
                LogService.Log(payService.CurrentPayment + "result or notify got an exception", e.ToString());
                responseCodeOrOrderId = payService.CurrentPayment + "result or notify got an exception";
            }

            return responseCodeOrOrderId;
        }
        
        /// <summary>
        /// Builds online payment service by name
        /// </summary>
        /// <param name="paymentName"></param>
        /// <returns></returns>
        private IPayService BuildPayService(string paymentName)
        {
            IPayService service = null;
            service = PayFactory.Create(
                PaymentService.ALIPAYSERVICE, 
                "2088711708147372", 
                "2i1dmrmyty1nnebyfmu8mmy41awfezmn", 
                "utf-8", 
                "http://" + StaticUtility.Config.Domain + "/m/pay/alipayresult",
                "http://" + StaticUtility.Config.Domain + "/m/pay/alipaynotify");

            return service;
        }

        /// <summary>
        /// Translates request result from online payment service async or sync request stream.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="isNotify"></param>
        /// <returns></returns>
        private NameValueCollection BuildRequestResult(IPayService service, bool isNotify)
        {
            NameValueCollection reqResult = new NameValueCollection();

            if (service.CurrentPayment.Equals(PaymentService.CHINABANKSERVICE, StringComparison.CurrentCultureIgnoreCase))
            {
                Request.ContentEncoding = Encoding.GetEncoding("GBK");
                reqResult = PayUtil.DecodeRequest(Request.InputStream);
            }
            else if (service.CurrentPayment.Equals(PaymentService.ALIPAYSERVICE, StringComparison.CurrentCultureIgnoreCase))
            {
                if (isNotify)
                {
                    reqResult = Request.Form;
                }
                else
                {
                    reqResult = Request.QueryString;
                }
            }
            else if (service.CurrentPayment.Equals(PaymentService.TENPAYSERVICE, StringComparison.CurrentCultureIgnoreCase))
            {
                reqResult = Request.QueryString;
            }
            else
            {
                // do nothing.
            }

            return reqResult;
        }
        
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
