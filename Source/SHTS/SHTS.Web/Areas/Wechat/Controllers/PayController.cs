using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Controllers;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Public;
using WitBird.Com.Pay;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class PayController : WechatBaseController
    {
        private static TenPayV3Info _tenPayV3Info;

        /// <summary>
        /// 微信支付相关配置信息
        /// </summary>
        public static TenPayV3Info TenPayV3Info
        {
            get
            {
                if (_tenPayV3Info == null)
                {
                    _tenPayV3Info =
                        TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
                }
                return _tenPayV3Info;
            }
        }

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
            bool isSuccessful = false;
            string errorMessage = "";

            string appId = string.Empty;
            string timeStamp = string.Empty;
            string nonceStr = string.Empty;
            string package = string.Empty;
            string paySign = string.Empty;

            try
            {
                TradeOrder order = orderService.GetOrderByOrderId(orderId);

                if (order == null)
                {
                    errorMessage = "订单信息有误，请重新尝试！";
                }
                else
                {
                    if (order.State == (int)OrderState.Succeed)
                    {
                        errorMessage = "订单已支付成功，请刷新页面后查看。";
                    }
                    else
                    {
                        isSuccessful = PreparePaySign(order, out appId, out timeStamp, out nonceStr, out package, out paySign, out errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("在线支付中介订单", ex.ToString());
                isSuccessful = false;
                errorMessage = "支付过程出错，请刷新页面后重新尝试！";
            }

            var jsonResult = new
            {
                IsSuccessFul = isSuccessful,
                Message = errorMessage,
                AppId = appId,
                TimeStamp = timeStamp,
                NonceStr = nonceStr,
                Package = package,
                PaySign = paySign
            };

            LogService.LogWexin("支付中介订单", "isSuccessFul: " + isSuccessful.ToString() + "  message:"
                + errorMessage + "  appId: " + appId + "  timeStamp:" + timeStamp + "  nonceStr:" + nonceStr + "  package:" + package + "  paySign:" + paySign);

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private bool PreparePaySign(TradeOrder order, out string appId, out string timeStamp,
            out string nonceStr, out string package, out string paySign, out string message)
        {
            bool isSuccessFul = false;

            appId = string.Empty;
            timeStamp = string.Empty;
            nonceStr = string.Empty;
            package = string.Empty;
            paySign = string.Empty;

            message = string.Empty;

            try
            {

                //创建支付应答对象
                RequestHandler packageReqHandler = new RequestHandler(null);
                //初始化
                packageReqHandler.Init();

                appId = TenPayV3Info.AppId;
                timeStamp = TenPayV3Util.GetTimestamp();
                nonceStr = TenPayV3Util.GetNoncestr();

                string body = "活动在线网-支付中介交易款项";

                //设置package订单参数
                packageReqHandler.SetParameter("appid", TenPayV3Info.AppId);		  //公众账号ID
                packageReqHandler.SetParameter("mch_id", TenPayV3Info.MchId);		  //商户号
                packageReqHandler.SetParameter("nonce_str", nonceStr);                    //随机字符串
                packageReqHandler.SetParameter("body", body);    //商品信息
                packageReqHandler.SetParameter("out_trade_no", order.OrderId);		//商家订单号
                packageReqHandler.SetParameter("total_fee", (Convert.ToInt32(order.Amount * 100).ToString()));			        //商品金额,以分为单位(money * 100).ToString()
                packageReqHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);   //用户的公网ip，不是商户服务器IP
                packageReqHandler.SetParameter("notify_url", TenPayV3Info.TenPayV3Notify);		    //接收财付通通知的URL
                packageReqHandler.SetParameter("trade_type", TenPayV3Type.JSAPI.ToString());	                    //交易类型
                packageReqHandler.SetParameter("openid", CurrentWeChatUser.OpenId);	                    //用户的openId

                string sign = packageReqHandler.CreateMd5Sign("key", TenPayV3Info.Key);
                packageReqHandler.SetParameter("sign", sign);	                    //签名

                string data = packageReqHandler.ParseXML();

                var result = TenPayV3.Unifiedorder(data);
                var res = XDocument.Parse(result);

                LogService.LogWexin("微信支付参数", res.ToString());
                string prepayId = res.Element("xml").Element("prepay_id").Value;

                //设置支付参数
                RequestHandler paySignReqHandler = new RequestHandler(null);
                paySignReqHandler.SetParameter("appId", TenPayV3Info.AppId);
                paySignReqHandler.SetParameter("timeStamp", timeStamp);
                paySignReqHandler.SetParameter("nonceStr", nonceStr);
                paySignReqHandler.SetParameter("package", string.Format("prepay_id={0}", prepayId));
                paySignReqHandler.SetParameter("signType", "MD5");

                paySign = paySignReqHandler.CreateMd5Sign("key", TenPayV3Info.Key);
                package = string.Format("prepay_id={0}", prepayId);
            }
            catch (Exception ex)
            {
                message = "支付过程发生错误";
                LogService.LogWexin("构建支付参数出错", ex.ToString());
            }

            if (!string.IsNullOrEmpty(appId) &&
                !string.IsNullOrEmpty(timeStamp) &&
                !string.IsNullOrEmpty(nonceStr) &&
                !string.IsNullOrEmpty(package) &&
                !string.IsNullOrEmpty(paySign))
            {
                isSuccessFul = true;
            }

            return isSuccessFul;
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
