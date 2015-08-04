using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WitBird.Com.SMS
{
    [Serializable]
    public class ShortMessage
    {
        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content { set; get; }

        /// <summary>
        /// 目标手机号码。
        /// </summary>
        public string ToPhoneNumber { set; get; }

        /// <summary>
        /// 短信模板编号（主流供应商方案，可选）
        /// </summary>
        public string TemplateId { set; get; }

        /// <summary>
        /// 如果是模板短信，需要提供的参数
        /// </summary>
        public string[] Parameters { set; get; }
    }
}
