namespace WitBird.Com.Pay.Alipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    internal static class Config
    {
        #region 字段
        private static string _input_charset = "";
        private static string _sign_type = "";
        private static string _gateway_new = "";
        private static string _https_veryfy_url = "";
        private static string _payment_type = "";
        private static string _service = "";
        private static string _returnUrl = "";
        private static string _notifyUrl = "";

        private static string _partner = "";
        private static string _key = "";
        private static string _seller_email = "";
        #endregion

        static Config()
        {
            //字符编码格式 目前支持 gbk 或 utf-8
            //_input_charset = "utf-8";
            //签名方式，选择项：RSA、DSA、MD5
            _sign_type = "MD5";
            //支付宝网关地址（新）
            _gateway_new = "https://mapi.alipay.com/gateway.do";
            //支付宝消息验证地址
            _https_veryfy_url = "https://mapi.alipay.com/gateway.do?service=notify_verify";
            //支付类型, 1:商品购买
            _payment_type = "1";
            _service = "create_direct_pay_by_user";
        }

        /// <summary>
        /// 初始化基础配置信息
        /// </summary>
        /// <param name="parter">合作身份者ID，以2088开头由16位纯数字组成的字符串</param>
        /// <param name="key">交易安全检验码，由数字和字母组成的32位字符串</param>
        /// <param name="seller_email">卖家支付宝帐户</param>
        public static void Initialize(string parter, string key, string inputCharset, string returnUrl, string notifyUrl)
        {
            _partner = parter;
            _key = key;
            _input_charset = inputCharset;
            _returnUrl = returnUrl;
            _notifyUrl = notifyUrl;
        }

        #region 属性

        public static string ReturnUrl
        {
            get { return _returnUrl; }
        }

        public static string NotifyUrl
        {
            get { return _notifyUrl; }
        }

        public static string Service
        {
            get { return _service; }
        }
        public static string Seller_email
        {
            get { return _seller_email; }
            set { _seller_email = value; }
        }

        /// <summary>
        /// 支付类型, 1:商品购买
        /// </summary>
        public static string Payment_type
        {
            get { return _payment_type; }
        }

        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Partner
        {
            get { return _partner; }
            set { _partner = value; }
        }

        /// <summary>
        /// 获取或设交易安全校验码
        /// </summary>
        public static string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset
        {
            get { return _input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type
        {
            get { return _sign_type; }
        }

        /// <summary>
        /// Get the gate way of Alipay.
        /// </summary>
        public static string Gateway
        {
            get { return _gateway_new; }
        }

        /// <summary>
        /// Get the verify url.
        /// </summary>
        public static string Https_veryfy_url
        {
            get { return _https_veryfy_url; }
        }
        #endregion
    }
}