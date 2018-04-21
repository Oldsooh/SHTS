namespace WitBird.Com.Pay.Tenpay
{
    internal class TenpayConfig
    {
        //private static string tenpay = "1";
        private static string _partner = "";
        private static string _tenpay_key = "";
        private static string _input_charset = "";
        private static string _returnUrl = "";
        private static string _notifyUrl = "";

        private static string _sign_type = "";
        private static string _gateway = "";
        private static string _https_verify_url = "";
        private static string _feeType;
        private static string _bankType;
        private static string _serviceVersion;
        private static string _sign_key_index;

        static TenpayConfig()
        {
            _sign_type = "MD5";
            _gateway = "https://gw.tenpay.com/gateway/pay.htm";
            _https_verify_url = "https://gw.tenpay.com/gateway/simpleverifynotifyid.xml";
            _feeType = "1"; // CNY
            _bankType = "DEFAULT";
            _serviceVersion = "1.0";
            _sign_key_index = "1";
        }

        public static void Initialize(string parter, string key, string inputCharset, string returnUrl, string notifyUrl)
        {
            _partner = parter;
            _tenpay_key = key;
            _input_charset = inputCharset;
            _returnUrl = returnUrl;
            _notifyUrl = notifyUrl;
        
        }

        public static string ReturnUrl
        {
            get { return _returnUrl; }
        }

        public static string NotifyUrl
        {
            get { return _notifyUrl; }
        }

        public static string SignKeyIndex
        {
            get { return _sign_key_index; }
        }

        public static string ServiceVersion
        {
            get { return _serviceVersion; }
        }

        public static string BankType
        {
            get { return _bankType; }
        }

        public static string FeeType
        {
            get { return _feeType; }
        }

        public static string PartnerId
        {
            get { return _partner; }
        }

        public static string TenpayKey
        {
            get { return _tenpay_key; }
        }

        public static string InputCharset
        {
            get { return _input_charset; }
        }

        public static string Gateway
        {
            get { return _gateway; }
        }

        public static string VerifyUrl
        {
            get { return _https_verify_url; }
        }

        public static string SignType
        {
            get { return _sign_type; }
        }
    }
}
