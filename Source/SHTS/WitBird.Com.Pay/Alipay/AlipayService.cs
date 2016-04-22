using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WitBird.Com.Pay.Alipay;

namespace WitBird.Com.Pay
{
    /// <summary>
    /// Simple alipay direct pay class, only UTF-8 and MD5 are supported.
    /// </summary>
    internal sealed class AlipayService : IPayService
    {
        private bool isInitialized = false;
        private Notify _notify;

        public AlipayService()
        {
            _notify = new Notify();
        }

        #region Implement IPayService interface

        /// <summary>
        /// <see cref="IPayService.IsInitialized"/>
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
        }

        /// <summary>
        /// <see cref="IPayService.CurrentPayment"/>
        /// </summary>
        public string CurrentPayment
        {
            get
            {
                return PaymentService.ALIPAYSERVICE;
            }
        }

        public string ResponseCodeSucceed
        {
            get { return PayResponseCode.SUCCESS; }
        }

        public string ResponseCodeFailed
        {
            get { return PayResponseCode.FAILED; }
        }

        /// <summary>
        /// <see cref="IPayService.Initialize"/>
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="key"></param>
        public void Initialize(string merchantId, string key, string inputCharset,
            string returnUrl, string notifyUrl)
        {
            Config.Initialize(merchantId, key, inputCharset, returnUrl, notifyUrl);
            isInitialized = true;
        }

        /// <summary>
        /// <see cref="IPayment.BuildRequest"/>
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="amount"></param>
        /// <param name="returnUrl"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="inputCharset"></param>
        /// <param name="otherParameters"></param>
        /// <returns></returns>
        public string BuildRequest(PayRequestCriteria criteria)
        {
            if (!isInitialized)
            {
                throw new System.ArgumentException("The Alipay payment configuration has not been initialized yet.");
            }

            if (criteria == null)
            {
                throw new System.ArgumentException("The criteria is null.");
            }

            CheckNullOrEmpty(criteria.OrderId);
            CheckNullOrEmpty(criteria.Subject);
            //CheckNullOrEmpty(criteria.Body);
            CheckAmount(criteria.Amount);

            SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();
            parameters.Add("partner", Config.Partner.Trim());
            parameters.Add("_input_charset", Config.Input_charset.Trim().ToLower());
            parameters.Add("service", Config.Service);
            parameters.Add("payment_type", Config.Payment_type);
            parameters.Add("notify_url", Config.NotifyUrl.Trim());
            parameters.Add("return_url", Config.ReturnUrl.Trim());
            parameters.Add("seller_email", Config.Seller_email);
            parameters.Add("seller_id", Config.Partner.Trim());
            parameters.Add("out_trade_no", criteria.OrderId);
            parameters.Add("total_fee", criteria.Amount);
            parameters.Add("subject", criteria.Subject);
            parameters.Add("body", criteria.Body);
            //parameters.Add("show_url", show_url);
            //parameters.Add("anti_phishing_key", Submit.Query_timestamp());
            parameters.Add("exter_invoke_ip", criteria.ClientIP);
            if (criteria.CustormParameters != null && criteria.CustormParameters.Count > 0)
            {
                foreach (var item in criteria.CustormParameters)
                {
                    if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    {
                        parameters.Add(item.Key.Trim(), item.Value.Trim());
                    }
                }
            }
            return Com.Pay.Alipay.Submit.BuildRequest(parameters, "get", "确认");
        }

        /// <summary>
        /// <see cref="IPayment.Verify"/>
        /// </summary>
        /// <param name="requestParamamters"></param>
        /// <returns></returns>
        public bool Verify(NameValueCollection requestParamamters)
        {
            if (!isInitialized)
            {
                throw new System.ArgumentException("The Alipay payment configuration has not been initialized yet.");
            }

            return _notify.Verify(requestParamamters);
        }

        /// <summary>
        /// <see cref="IPayService.ParseResult"/>
        /// </summary>
        /// <param name="requestParamamters"></param>
        /// <returns></returns>
        public PayResult ParseResult(NameValueCollection requestParamamters)
        {
            PayResult result = null;

            if (requestParamamters != null && requestParamamters.Count > 0)
            {
                result = new PayResult();
                result.OrderId = requestParamamters["out_trade_no"];

                decimal amount = 0;
                decimal.TryParse(requestParamamters["total_fee"], out amount);
                result.Amount = amount;

                string status = requestParamamters["trade_status"];
                if (status.Equals("TRADE_SUCCESS") || status.Equals("TRADE_FINISHED"))
                {
                    result.TradeStatus = PayStatus.Success;
                }
                else if (status.Equals("WAIT_BUYER_PAY") || status.Equals("TRADE_CLOSED") || status.Equals("TRADE_PENDING"))
                {
                    result.TradeStatus = PayStatus.Failed;
                }
                else
                {
                    result.TradeStatus = PayStatus.UnKnow;
                }

                result.OrderSubject = requestParamamters["subject"];
                result.OrderBody = requestParamamters["body"];
            }

            return result;
        }

        #endregion Implement IPayService interface

        private static void CheckNullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("The required paramter is null");
            }
        }

        private static void CheckAmount(string amount)
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
                    throw new ArgumentException("Two points only for amount.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid amount");
            }
        }
    }
}