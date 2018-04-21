using System.Collections.Generic;
using System.Linq;

namespace WitBird.Com.Pay
{
    /// <summary>
    /// Pay request criteria.
    /// </summary>
    public class PayRequestCriteria
    {
        private string orderId;
        private string amount;
        private string subject;
        private string body;
        private string clientIPAddr;
        private Dictionary<string, string> otherParameters;

        public PayRequestCriteria()
        {
            otherParameters = new Dictionary<string, string>();
        }
        
        /// <summary>
        /// Gets or sets the unique order id in merchant database.
        /// </summary>
        public string OrderId
        {
            get { return orderId; }
            set { this.orderId = value; }
        }

        /// <summary>
        /// Gets or sets the amount in trade. Type: CNY(RMB); Unit: Yuan. Two points only.
        /// </summary>
        public string Amount
        {
            get { return this.amount; }
            set { this.amount = value; }
        }

        /// <summary>
        /// Gets or set the order subject.
        /// </summary>
        public string Subject
        {
            get { return this.subject; }
            set { this.subject = value; }
        }

        /// <summary>
        /// Gets or sets the order body.
        /// </summary>
        public string Body
        {
            get { return this.body; }
            set { this.body = value; }
        }

        /// <summary>
        /// Gets or set the client IP address which post this online pay request.
        /// </summary>
        public string ClientIP
        {
            get { return this.clientIPAddr; }
            set { this.clientIPAddr = value; }
        }

        /// <summary>
        /// Gets the other custom parameters.
        /// </summary>
        public Dictionary<string, string> CustormParameters
        {
            get { return this.otherParameters; }
        }

        /// <summary>
        /// Adds a new custom parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddCustomParameter(string name, string value)
        {
            if (otherParameters.Keys.Contains(name))
            {
                otherParameters.Remove(name);
            }

            otherParameters.Add(name, value);
        }
    }
}
