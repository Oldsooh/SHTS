using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WitBird.Com.Pay.Tenpay;

namespace WitBird.Com.Pay
{
    /// <summary>
    /// Tenpay service class.
    /// </summary>
    internal sealed class TenpayService : IPayService
    {
        private bool isInitialized = false;

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public void Initialize(string merchantId, string key, string inputCharset,
            string returnUrl, string notifyUrl)
        {
            TenpayConfig.Initialize(merchantId, key, inputCharset, returnUrl, notifyUrl);
            isInitialized = true;
        }

        /// <summary>
        /// <see cref="IPayService.CurrentPayment"/>
        /// </summary>
        public string CurrentPayment
        {
            get
            {
                return PaymentService.TENPAYSERVICE;
            }
        }

        public string BuildRequest(PayRequestCriteria criteria)
        {
            if (!isInitialized)
            {
                throw new ArgumentException("The Tenpay payment configuration has not been initialized yet.");
            }

            if (criteria == null)
            {
                throw new System.ArgumentException("The criteria is null.");
            }

            CheckNullOrEmpty("OrderId", criteria.OrderId);
            CheckNullOrEmpty("Subject", criteria.Subject);
            //CheckNullOrEmpty("Body", criteria.Body);

            string amount = criteria.Amount;
            ParseAmountFromYuanToFen(ref amount);

            //创建RequestHandler实例
            RequestHandler reqHandler = new RequestHandler();

            //-----------------------------
            //设置支付参数
            //-----------------------------
            reqHandler.setParameter("partner", TenpayConfig.PartnerId.Trim());		        //商户号
            reqHandler.setParameter("out_trade_no", criteria.OrderId);		//商家订单号
            reqHandler.setParameter("total_fee", amount);			        //商品金额,以分为单位
            reqHandler.setParameter("return_url", TenpayConfig.ReturnUrl.Trim());		    //交易完成后跳转的URL
            reqHandler.setParameter("notify_url", TenpayConfig.NotifyUrl.Trim());		    //接收财付通通知的URL
            reqHandler.setParameter("bank_type", TenpayConfig.BankType.Trim());		    //银行类型(中介担保时此参数无效)
            reqHandler.setParameter("body", criteria.Body);
            reqHandler.setParameter("subject", criteria.Subject);
            reqHandler.setParameter("spbill_create_ip", criteria.ClientIP);
            reqHandler.setParameter("fee_type", TenpayConfig.FeeType);

            //系统可选参数
            reqHandler.setParameter("sign_type", TenpayConfig.SignType);
            reqHandler.setParameter("service_version", TenpayConfig.ServiceVersion);
            reqHandler.setParameter("input_charset", TenpayConfig.InputCharset.Trim().ToUpper());
            reqHandler.setParameter("sign_key_index", TenpayConfig.SignKeyIndex);

            //string attach = "[subject=" + criteria.Subject + "][body=" + criteria.Body;
            //if (attach.Length >= 127)
            //{
            //    attach = attach.Substring(0, 125);
            //}

            //attach += "]";

            //reqHandler.setParameter("attach", attach);

            // Other parameters
            if (criteria.CustormParameters != null && criteria.CustormParameters.Count > 0)
            {
                foreach (var item in criteria.CustormParameters)
                {
                    reqHandler.setParameter(item.Key, item.Value);
                }
            }

            // create sign here
            reqHandler.createSign();

            //post实现方式
            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='chinabankpaymentsubmit' name='tenpaysubmit' action='" + TenpayConfig.Gateway.Trim() + "' method='post'>");

            Hashtable ht = reqHandler.getAllParameters();
            foreach (DictionaryEntry de in ht)
            {
                sbHtml.Append(BuildHtml(de.Key.ToString(), de.Value.ToString()));
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='确认付款' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['tenpaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }

        public bool Verify(System.Collections.Specialized.NameValueCollection requestParamamters)
        {
            if (!isInitialized)
            {
                throw new ArgumentException("The Tenpay payment configuration has not been initialized yet.");
            }

            bool isValid = false;

            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(requestParamamters);
            resHandler.setKey(TenpayConfig.TenpayKey);

            //判断签名
            if (resHandler.isTenpaySign())
            {
                ///通知id
                string notify_id = resHandler.getParameter("notify_id");
                //通过通知ID查询，确保通知来至财付通
                //创建查询请求
                RequestHandler queryReq = new RequestHandler();
                queryReq.init();
                queryReq.setGateUrl(TenpayConfig.VerifyUrl);
                queryReq.setParameter("partner", TenpayConfig.PartnerId);
                queryReq.setParameter("notify_id", notify_id);

                //通信对象
                TenpayHttpClient httpClient = new TenpayHttpClient();
                httpClient.setTimeOut(10);
                //设置请求内容
                httpClient.setReqContent(queryReq.getRequestURL());
                //后台调用
                if (httpClient.call())
                {
                    //设置结果参数
                    ClientResponseHandler queryRes = new ClientResponseHandler();
                    queryRes.setContent(httpClient.getResContent());
                    queryRes.setKey(TenpayConfig.TenpayKey);
                    //判断签名及结果
                    if (queryRes.isTenpaySign())
                    {
                        isValid = true;
                    }
                }
            }

            return isValid;
        }

        public PayResult ParseResult(System.Collections.Specialized.NameValueCollection requestParamamters)
        {
            PayResult result = null;

            if (requestParamamters != null && requestParamamters.Count > 0)
            {
                result = new PayResult();

                //取结果参数做业务处理
                string out_trade_no = requestParamamters["out_trade_no"];

                //金额,以分为单位
                string total_fee = requestParamamters["total_fee"];
                //支付结果
                string trade_state = requestParamamters["trade_state"];

                result.OrderId = out_trade_no;
                result.Amount = ParseAmountFromFenToYuan(total_fee);

                if ("0".Equals(trade_state))
                {
                    result.TradeStatus = PayStatus.Success;
                }
                else
                {
                    result.TradeStatus = PayStatus.Failed;
                }
                result.OrderSubject = string.Empty;
                result.OrderBody = string.Empty;
                ////("attach", "[subject=" + subject + "][body=" + body + "]")
                //string attach = requestParamamters["attach"];
                //string[] temps;

                //if (attach != string.Empty)
                //{
                //    temps = attach.Split(new string[] { "][" }, StringSplitOptions.None);
                //    if (temps != null && temps.Length > 0)
                //    {
                //        foreach (var item in temps)
                //        {
                //            if (!string.IsNullOrEmpty(item))
                //            {
                //                if (item.IndexOf("[subject=") > -1)
                //                {
                //                    result.OrderSubject = item.Replace("[subject=", string.Empty);
                //                }
                //                else if (item.IndexOf("body=") > -1)
                //                {
                //                    result.OrderBody = item.Replace("body=", string.Empty);
                //                    result.OrderBody = result.OrderBody.Remove(result.OrderBody.Length - 1);
                //                }
                //                else
                //                {
                //                }
                //            }
                //        }
                //    }
                //}
            }

            return result;
        }

        private static void CheckNullOrEmpty(string parameterName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("The required paramter \"" + parameterName + "\" is null or empty");
            }
        }

        /// <summary>
        /// Yuan to Fen
        /// </summary>
        /// <param name="amount"></param>
        private static void ParseAmountFromYuanToFen(ref string amount)
        {
            if (string.IsNullOrEmpty(amount))
            {
                throw new ArgumentNullException("The required amount is null");
            }

            decimal _amount;

            if (decimal.TryParse(amount, out _amount))
            {
                int dotIndex = amount.IndexOf('.');
                if (dotIndex > -1 && amount.Substring(dotIndex + 1).Length > 2)
                {
                    throw new ArgumentException("Invalid amount");
                }
            }
            else
            {
                throw new ArgumentException("Invalid amount");
            }

            amount = ((int)(_amount * 100)).ToString();
        }

        private static decimal ParseAmountFromFenToYuan(string amount)
        {
            if (string.IsNullOrEmpty(amount))
            {
                throw new ArgumentNullException("The required amount is null");
            }

            int _amount = 0;
            decimal _dAmount = 0;

            if (int.TryParse(amount, out _amount))
            {
                _dAmount = decimal.Parse(amount) / 100;
            }
            else
            {
                throw new ArgumentException("Invalid amount");
            }

            return _dAmount;
        }

        private static string BuildHtml(string key, string value)
        {
            return "<input type='hidden' name='" + key + "' value='" + value + "'/>";
        }

        public string ResponseCodeSucceed
        {
            get { return PayResponseCode.SUCCESS; }
        }

        public string ResponseCodeFailed
        {
            get { return PayResponseCode.FAIL; }
        }
    }
}
