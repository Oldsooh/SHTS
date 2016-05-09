using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Witbird.SHTS.Model
{
    public class MailConfig
    {
        /// <summary>
        /// 邮件发送服务器
        /// </summary>
        public string EmailServer { get; set; }

        /// <summary>
        /// 发件人邮箱帐号, 如xxx@163.com
        /// </summary>
        public string MailAccount { get; set; }

        /// <summary>
        /// 发件人账号, 如发件人邮箱是xxx@163.com，则发件人账号为xxx
        /// </summary>
        public string MailAccountName { get; set; }

        /// <summary>
        /// 发件人邮箱密码
        /// </summary>
        public string MailAccountPassword { get; set; }

        /// <summary>
        /// 邮件服务器发送端口，默认为25
        /// </summary>
        public int EmailServerPort { get; set; }

        /// <summary>
        /// 是否启用SSL协议对邮件内容进行加密
        /// </summary>
        public bool EnableSSL { get; set; }

        /// <summary>
        /// 是否在发送邮件对发件人邮箱进行密码校验
        /// </summary>
        public bool EnableAuthentication { get; set; }
    }
}