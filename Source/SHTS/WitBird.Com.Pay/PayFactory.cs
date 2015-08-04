using System;

namespace WitBird.Com.Pay
{
    public class PayFactory
    {
        /// <summary>
        /// Creates a specified online payment interface instance.
        /// </summary>
        /// <typeparam name="T">The type of payment interface.</typeparam>
        /// <param name="merchantId">The unique merchant id in online bank's database which provides the online payment interface.</param>
        /// <param name="key">The key which used to request a trade with the merchant id.</param>
        /// <param name="returnUrl">The sync return url when pay completed. If user close payment page directly, this return page should not be displayed.</param>
        /// <param name="notifyUrl">The async notify url when pay completed. This return page should be called in asynchronous by payment service.</param>
        /// <returns>Returns a spcified online payment service.</returns>
        /// <exception cref="System.ArgumentException">Parameter is null or empty.</exception>
        public static IPayService Create(string payment, string merchantId, string key, string inputCharset, string returnUrl, string notifyUrl)
        {
            ParameterCheck("merchantId", merchantId);
            ParameterCheck("key", key);
            ParameterCheck("inputCharset", inputCharset);
            ParameterCheck("returnUrl", returnUrl);
            ParameterCheck("notifyUrl", notifyUrl);

            IPayService payService = null;
            
            switch (payment)
            {
                case PaymentService.ALIPAYSERVICE:
                    payService = Activator.CreateInstance<AlipayService>();
                    break;
                case PaymentService.CHINABANKSERVICE:
                    payService = Activator.CreateInstance<ChinabankService>();
                    break;
                case PaymentService.TENPAYSERVICE:
                    payService = Activator.CreateInstance<TenpayService>();
                    break;
                default:
                    throw new ArgumentException("The online payment service " + payment +" does not support in this system.");
            }

            if (payService != null)
            {
                payService.Initialize(merchantId, key, inputCharset, returnUrl, notifyUrl);
            }

            return payService;
        }

        private static void ParameterCheck(string parameterName, string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim()))
            {
                throw new ArgumentException("The value for " + parameterName + " is null or empty");
            }
        }
    }
}
