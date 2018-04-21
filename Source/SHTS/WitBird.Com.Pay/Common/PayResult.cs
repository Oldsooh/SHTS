using System;
using System.Collections.Generic;

namespace WitBird.Com.Pay
{
    /// <summary>
    /// Trade result.
    /// </summary>
    public class PayResult
    {
        private string orderId;
        private decimal amount;
        private string subject;
        private string body;
        private PayStatus tradeStatus;
        private Dictionary<string, string> customParameters;

        public PayResult()
        {
            orderId = string.Empty;
            amount = 0;
            subject = string.Empty;
            body = string.Empty;
            tradeStatus = Pay.PayStatus.UnKnow;
            customParameters = new Dictionary<string, string>();
        }

        #region Properties

        /// <summary>
        /// Gets or internal sets the order id.
        /// </summary>
        public string OrderId
        {
            get
            {
                return this.orderId;
            }
            internal set
            {
                this.orderId = value.Trim();
            }
        }

        /// <summary>
        /// Gets or internal sets the Amount.
        /// </summary>
        /// <exception cref="System.ArgumentException">Amount is invalid.</exception>
        public decimal Amount
        {
            get
            {
                return this.amount;
            }
            internal set
            {
                //if (value < 0)
                //{
                //    throw new ArgumentException("Amount should equal or greater than 0.");
                //}

                //int dotIndex = value.ToString().IndexOf('.');

                //if (dotIndex > -1 && value.ToString().Substring(dotIndex + 1).Length > 2)
                //{
                //    throw new ArgumentException("Mostlty two points only for Amount.");
                //}
                
                this.amount = value;
            }
        }

        /// <summary>
        /// Gets or internal sets the trade status.
        /// </summary>
        public PayStatus TradeStatus
        {
            get
            {
                return this.tradeStatus;
            }
            internal set
            {
                this.tradeStatus = value;
            }
        }

        /// <summary>
        /// Gets or interal sets the order subject.
        /// </summary>
        public string OrderSubject
        {
            get
            {
                return this.subject;
            }
            internal set
            {
                this.subject = value;
            }
        }

        /// <summary>
        /// Gets or internal sets the order body.
        /// </summary>
        public string OrderBody
        {
            get
            {
                return this.body;
            }
            internal set
            {
                this.body = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the specified customer parameters for different online payment service.
        /// </summary>
        public string TryGetParameterValue(string parameterName)
        {
            string value = string.Empty;

            if (!string.IsNullOrEmpty(parameterName) &&
                customParameters != null && 
                customParameters.Count > 0 &&
                customParameters.ContainsKey(parameterName))
            {
                value = customParameters[parameterName];
            }

            return value;
        }

        /// <summary>
        /// Adds the specified custom parameter to TradeResult.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentException">The parameter already exists.</exception>
        public void AddCustomParameter(string parameterName, string value)
        {
            if (!string.IsNullOrEmpty(parameterName) && 
                !string.IsNullOrEmpty(parameterName.Trim()) &&
                !string.IsNullOrEmpty(value) &&
                !string.IsNullOrEmpty(value.Trim()))
            {
                if (!customParameters.ContainsKey(parameterName))
                {
                    customParameters.Add(parameterName, value);
                }
                else
                {
                    throw new ArgumentException("The specified parameter alreay exists.");
                }
            }
        }

        /// <summary>
        /// Removes the specified custom parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        public void RemoveCustomParameter(string parameterName)
        {
            if (!string.IsNullOrEmpty(parameterName) &&
                customParameters.ContainsKey(parameterName))
            {
                customParameters.Remove(parameterName);
            }
        }

        /// <summary>
        /// Overrides ToString() to output formatted result.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "Order ID: " + orderId + "\r\n" +
                "Amount: " + amount + "\r\n" +
                "Trade Status: " + tradeStatus + "\r\n" +
                "Order subject: " + subject + "\r\n" +
                "Order body: " + body + "\r\n";

            if (customParameters != null && customParameters.Count > 0)
            {
                foreach (var item in customParameters)
                {
                    result += item.Key + ": " + item.Value + "\r\n";
                }
            }

            return result;
        }

        #endregion
    }
}
