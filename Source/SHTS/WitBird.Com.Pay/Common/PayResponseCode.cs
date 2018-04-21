namespace WitBird.Com.Pay
{
    /// <summary>
    /// Defines some const response text for different online payment service.
    /// If online service receieved success info, it will not post notify again.
    /// </summary>
    public static class PayResponseCode
    {
        /// <summary>
        /// Success, Alipay and Tenpay use.
        /// </summary>
        public const string SUCCESS = "success";

        /// <summary>
        /// Failed, Alipay use.
        /// </summary>
        public const string FAILED = "failed";

        /// <summary>
        /// Fail, Tenpay use.
        /// </summary>
        public const string FAIL = "fail";

        /// <summary>
        /// Ok, Chinabank use.
        /// </summary>
        public const string OK = "ok";

        /// <summary>
        /// Error, Chinabank use.
        /// </summary>
        public const string ERROR = "error";
    }
}
