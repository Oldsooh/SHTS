using System;

namespace WitBird.Com.SMS
{
    /// <summary>
    /// 发送返回信息。
    /// </summary>
    public class SMSResponse
    {
        private const string SuccessCode = "200";

        /// <summary>
        /// 状态码.
        /// </summary>
        public string statusCode { set; get; }

        /// <summary>
        /// 返回数据.
        /// </summary>
        public string ResponseData { set; get; }

        public Exception InnerException { get; set; }

        public bool IsSuccess
        {
            get
            {
                return  string.Equals(this.statusCode,SuccessCode);
            }
        }
    }
}
