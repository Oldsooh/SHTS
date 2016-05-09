using System;

namespace Witbird.SHTS.Common
{
    public static class Constant
	{
        /// <summary>
        /// 跳转到支付页面，需要两个参数，1: OrderId, 2:returnUrl
        /// </summary>
        public const string PostPayInfoFormat = @"正在跳转...<br />如果浏览器没有自动跳转，请点击
                                                 <a href='javascript:void(0);' target='_parent' title='跳转到订单支付页面' onclick=javascript:document.getElementById('payForm').submit();>跳转到订单支付页面</a>
                                                 <form id='payForm' action='/pay/GenerateOrder' method='post'>
                                                       <input type='hidden' name='orderId' value='{0}' />
                                                       <input type='hidden' name='returnUrl' value='{1}' />
                                                 </form>
                                                 <script>document.getElementById('payForm').submit();</script>";

        /// <summary>
        /// 跳转到支付页面，需要两个参数，1: OrderId, 2:returnUrl
        /// </summary>
        public const string PostPayInfoFormatForMobile = @"正在跳转...<br />如果浏览器没有自动跳转，请点击
                                                 <a href='javascript:void(0);' target='_parent' title='跳转到订单支付页面' onclick=javascript:document.getElementById('payForm').submit();>跳转到订单支付页面</a>
                                                 <form id='payForm' action='/m/pay/GenerateOrder' method='post'>
                                                       <input type='hidden' name='orderId' value='{0}' />
                                                       <input type='hidden' name='returnUrl' value='{1}' />
                                                 </form>
                                                 <script>document.getElementById('payForm').submit();</script>";

        /// <summary>
        /// 后台配置中介交易线下银行付款帐号
        /// </summary>
        public const string TradeBankInfoConfig = "ActivityOnlineBankInfoConfig";

        public const char TradeBankInfoConfigSeperator = '^';

        #region 邮箱设置

        /// <summary>
        /// 邮件发送服务器
        /// </summary>
        public const string EmailServerHostName = "EmailServerHostName";

        /// <summary>
        /// 发件人邮箱帐号, 如xxx@163.com
        /// </summary>
        public const string MailAccount = "MailAccount";

        /// <summary>
        /// 发件人账号, 如发件人邮箱是xxx@163.com，则发件人账号为xxx
        /// </summary>
        public const string MailAccountName = "MailAccountName";

        /// <summary>
        /// 发件人邮箱密码
        /// </summary>
        public const string MailAccountPassword = "MailAccountPassword";

        /// <summary>
        /// 邮件服务器发送端口，默认为25
        /// </summary>
        public const string EmailServerPort = "EmailServerPort";

        /// <summary>
        /// 是否启用SSL协议对邮件内容进行加密
        /// </summary>
        public const string EnableSSL = "EnableSSL";

        /// <summary>
        /// 是否在发送邮件对发件人邮箱进行密码校验
        /// </summary>
        public const string EnableAuthentication = "EnableAuthentication";


        #endregion
    }
}