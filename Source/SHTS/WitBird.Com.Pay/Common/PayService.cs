namespace WitBird.Com.Pay
{
    /// <summary>
    /// Supported online payment service type.
    /// </summary>
    public class PaymentService
    {
        /// <summary>
        /// Alipay online pay service
        /// </summary>
        public const string ALIPAYSERVICE = "alipay";

        /// <summary>
        /// Tenpay online pay service
        /// </summary>
        public const string TENPAYSERVICE = "tenpay";

        /// <summary>
        /// Chinabank online pay service
        /// </summary>
        public const string CHINABANKSERVICE = "chinabank";
    }
}
