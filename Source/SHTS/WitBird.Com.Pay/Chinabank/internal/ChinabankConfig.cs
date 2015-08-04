
namespace WitBird.Com.Pay.Chinabank
{
    /// <summary>
    /// Configuration class.
    /// </summary>
    internal static class ChinabankConfig
    {
        private static string _v_mid = string.Empty;
        private static string _key = string.Empty;
        private static string _gateway = string.Empty;
        private static string _moneytype = string.Empty;
        private static string _inputCharset = string.Empty;
        private static string _returnUrl = "";
        private static string _notifyUrl = "";

        static ChinabankConfig()
        {
            _gateway = "https://pay3.chinabank.com.cn/PayGate";
            _moneytype = "CNY";
        }
        
        /// <summary>
        /// Initializes Chinabank payment configuration.
        /// </summary>
        /// <param name="v_mid"></param>
        /// <param name="key"></param>
        public static void Initialize(string v_mid, string key, string inputCharset, string returnUrl, string notifyUrl)
        {
            _v_mid = v_mid;
            _key = key;
            _inputCharset = inputCharset;
            _returnUrl = returnUrl;
            _notifyUrl = notifyUrl;
        }

        public static string ReturnUrl
        {
            get { return _returnUrl; }
        }

        public static string NotifyUrl
        {
            get 
            {
                return "[url:=" + _notifyUrl + "]";
            }
        }

        /// <summary>
        /// Gets the unique merchant id of ChinabankPayment.
        /// </summary>
        public static string V_Mid
        {
            get { return _v_mid; }
        }

        /// <summary>
        /// Gets the MD5 key of merchant.
        /// </summary>
        public static string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the pay gateway of Chinabank payment.
        /// </summary>
        public static string Gateway
        {
            get { return _gateway; }
        }

        /// <summary>
        /// Gets the money type for current trade. Default as "CNY".
        /// </summary>
        public static string MoneyType
        {
            get { return _moneytype; }
        }

        public static string InputCharset
        {
            get { return _inputCharset.Trim().ToUpper(); }
        }
    }
}
