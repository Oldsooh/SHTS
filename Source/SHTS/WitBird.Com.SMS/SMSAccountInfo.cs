using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Witbird.SHTS.Common;

namespace WitBird.Com.SMS
{
    [Serializable]
    public class SMSAccountInfo
    {
        #region 属性

        /// <summary>
        /// 运营商分配的账户ID.
        /// </summary>
        public string AccountSID { set; get; }

        /// <summary>
        /// 运营商分配的账户Authkey.
        /// </summary>
        public string AuthToken { set; get; }

        /// <summary>
        /// 运营商分配的请求IP.
        /// </summary>
        public string SendAdress { set; get; }

        /// <summary>
        /// 其他账户信息.
        /// </summary>
        public Dictionary<string, string> CustomizedInfos { set; get; }

        #endregion

        #region Singleton

        /// <summary>
        /// The locker for initializing singleton instance.
        /// </summary>
        static readonly object locker = new object();

        /// <summary>
        /// The instance for singleton.
        /// </summary>
        private static SMSAccountInfo instance;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>The instance.</value>
        public static SMSAccountInfo Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new SMSAccountInfo();
                        }
                    }
                }
                return instance;
            }
            private set { instance = value; }
        }

        #endregion

        /// <summary>
        /// 默认.
        /// </summary>
        /// <param name="accountsID"></param>
        /// <param name="authtoken"></param>
        /// <param name="customizedinfos"></param>
        public void Initialize(string accountsID,
            string authtoken, Dictionary<string, string> customizedinfos)
        {
            AccountSID = accountsID;
            AuthToken = authtoken;
            CustomizedInfos = customizedinfos;
        }

        /// <summary>
        /// 通过json配置文件。
        /// </summary>
        /// <param name="SMSAccountInfoJson"></param>
        public static void Initialize(SMSAccountInfo AccountInfo)
        {
            Instance = AccountInfo;
        }
    }
}
