using System;
using System.Collections.Specialized;
using System.Text;
using WitBird.Com.Pay.Chinabank;

namespace WitBird.Com.Pay
{
    /// <summary>
    /// Simple Chinabank payment class.
    /// </summary>
    internal sealed class ChinabankService : IPayService
    {
        private string v_mid = "";
        private string v_key = "";
        private string v_moneytype = "";
        private string _inputCharset = "";
        private bool isInitialized = false;

        public ChinabankService()
        {
        }

        #region Implement IPayService interface

        /// <summary>
        /// <see cref="IPayService.IsInitialized"/>
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return this.IsInitialized;
            }
        }

        /// <summary>
        /// <see cref="IPayService.CurrentPayment"/>
        /// </summary>
        public string CurrentPayment
        {
            get
            {
                return PaymentService.CHINABANKSERVICE;
            }
        }

        /// <summary>
        /// <see cref="IPayService.Initialize"/>
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="key"></param>
        public void Initialize(string merchantId, string key, string inputCharset,
            string returnUrl, string notifyUrl)
        {
            ChinabankConfig.Initialize(merchantId, key, inputCharset, returnUrl, notifyUrl);

            v_mid = ChinabankConfig.V_Mid.Trim();
            v_key = ChinabankConfig.Key.Trim();
            v_moneytype = ChinabankConfig.MoneyType.Trim().ToUpper();
            _inputCharset = ChinabankConfig.InputCharset.Trim();
            isInitialized = true;
        }

        /// <summary>
        /// <see cref="IPayment.BuildRequest"/>
        /// </summary>
        /// <returns></returns>
        public string BuildRequest(PayRequestCriteria criteria)
        {
            if (!isInitialized)
            {
                throw new ArgumentException("The Chinabank payment configuration has not been initialized yet.");
            }

            if (criteria == null)
            {
                throw new System.ArgumentException("The criteria is null.");
            }

            CheckNullOrEmpty(criteria.OrderId);
            CheckNullOrEmpty(criteria.Subject);
            //CheckNullOrEmpty(criteria.Body);
            CheckAmount(criteria.Amount);

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='chinabankpaymentsubmit' name='chinabankpaymentsubmit' action='" + ChinabankConfig.Gateway.Trim() + "?encoding="
                + _inputCharset.Trim().ToUpper() + "' method='post'>");

            // Append sign
            string prestr = criteria.Amount + v_moneytype + criteria.OrderId + v_mid + ChinabankConfig.ReturnUrl.Trim();
            string v_md5info = ChinabankPaymentMD5.Sign(prestr, v_key, _inputCharset.Trim()).ToUpper();
            sbHtml.Append(BuildHtml("v_md5info", v_md5info));

            sbHtml.Append(BuildHtml("v_mid", v_mid));
            sbHtml.Append(BuildHtml("v_oid", criteria.OrderId));
            sbHtml.Append(BuildHtml("v_amount", criteria.Amount));
            sbHtml.Append(BuildHtml("v_moneytype", v_moneytype));
            sbHtml.Append(BuildHtml("v_url", ChinabankConfig.ReturnUrl.Trim()));
            sbHtml.Append(BuildHtml("remark1", "[subject=" + criteria.Subject + "][body=" + criteria.Body + "]"));
            sbHtml.Append(BuildHtml("remark2", ChinabankConfig.NotifyUrl.Trim()));

            if (criteria.CustormParameters != null && criteria.CustormParameters.Count > 0)
            {
                foreach (var item in criteria.CustormParameters)
                {
                    if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    {
                        sbHtml.Append(BuildHtml(item.Key.Trim(), item.Value.Trim()));
                    }
                }
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='确认付款' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['chinabankpaymentsubmit'].submit();</script>");

            return sbHtml.ToString();
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
                throw new ArgumentException("The Chinabank payment configuration has not been initialized yet.");
            }

            bool isValid = false;

            if (requestParamamters != null && requestParamamters.Count > 0)
            {
                string v_oid = requestParamamters["v_oid"];
                string v_pstatus = requestParamamters["v_pstatus"];
                string v_amount = requestParamamters["v_amount"];
                string v_md5str = requestParamamters["v_md5str"];

                //v_oid，v_pstatus，v_amount，v_moneytype，key
                string prestr = v_oid + v_pstatus + v_amount + ChinabankConfig.MoneyType.ToUpper();

                isValid = ChinabankPaymentMD5.Verify(prestr, v_md5str, v_key, _inputCharset.Trim().ToUpper());
            }

            return isValid;
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
                result.OrderId = requestParamamters["v_oid"];

                decimal amount = 0;
                decimal.TryParse(requestParamamters["v_amount"], out amount);
                result.Amount = amount;

                string status = requestParamamters["v_pstatus"];

                if (status.Equals("20"))
                {
                    result.TradeStatus = PayStatus.Success;
                }
                else if (status.Equals("30"))
                {
                    result.TradeStatus = PayStatus.Failed;
                }
                else
                {
                    result.TradeStatus = PayStatus.UnKnow;
                }

                //("remark1", "[subject=" + subject + "][body=" + body + "]")
                string remark1 = requestParamamters["remark1"];
                string[] temps;

                if (remark1 != string.Empty)
                {
                    temps = remark1.Split(new string[] { "][" }, StringSplitOptions.None);
                    if (temps != null && temps.Length > 0)
                    {
                        foreach (var item in temps)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (item.IndexOf("[subject=") > -1)
                                {
                                    result.OrderSubject = item.Replace("[subject=", string.Empty);
                                }
                                else if (item.IndexOf("body=") > -1)
                                {
                                    result.OrderBody = item.Replace("body=", string.Empty);
                                    result.OrderBody = result.OrderBody.Remove(result.OrderBody.Length - 1);
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        #endregion Implement IPayService interface

        #region Private methods

        private static string BuildHtml(string key, string value)
        {
            return "<input type='hidden' name='" + key + "' value='" + value + "'/>";
        }

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
                    throw new ArgumentException("Invalid amount");
                }
            }
            else
            {
                throw new ArgumentException("Invalid amount");
            }
        }

        #endregion End private methods


        public string ResponseCodeSucceed
        {
            get { return PayResponseCode.OK; }
        }

        public string ResponseCodeFailed
        {
            get { return PayResponseCode.ERROR; }
        }
    }
}
