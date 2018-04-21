using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace WitBird.Com.Pay.Tenpay
{
    /// <summary>
    /// ResponseHandler 的摘要说明。
    /// </summary>
    internal class ResponseHandler
    {
        /** 密钥 */
        private string key;

        /** 应答的参数 */
        protected Hashtable parameters;

        //获取服务器通知数据方式，进行参数获取
        public ResponseHandler(NameValueCollection collection)
        {
            parameters = new Hashtable();

            if (collection != null && collection.Count > 0)
            {
                foreach (string k in collection)
                {
                    string v = (string)collection[k];
                    this.setParameter(k, v);
                }
            }
        }

        /** 获取密钥 */
        public string getKey()
        { return key; }

        /** 设置密钥 */
        public void setKey(string key)
        { this.key = key; }

        /** 获取参数值 */
        public string getParameter(string parameter)
        {
            string s = (string)parameters[parameter];
            return (null == s) ? "" : s;
        }

        /** 设置参数值 */
        public void setParameter(string parameter, string parameterValue)
        {
            if (parameter != null && parameter != "")
            {
                if (parameters.Contains(parameter))
                {
                    parameters.Remove(parameter);
                }

                parameters.Add(parameter, parameterValue);
            }
        }

        /** 是否财付通签名,规则是:按参数名称a-z排序,遇到空值的参数不参加签名。 
         * @return boolean */
        public virtual Boolean isTenpaySign()
        {
            StringBuilder sb = new StringBuilder();

            ArrayList akeys = new ArrayList(parameters.Keys);
            akeys.Sort();

            foreach (string k in akeys)
            {
                string v = (string)parameters[k];
                if (null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + v + "&");
                }
            }

            sb.Append("key=" + this.getKey());
            string sign = MD5Util.GetMD5(sb.ToString(), TenpayConfig.InputCharset).ToLower();

            return getParameter("sign").ToLower().Equals(sign);
        }

        /** 是否财付通签名,规则是:按参数名称a-z排序,遇到空值的参数不参加签名。 
         * @return boolean */
        public virtual Boolean _isTenpaySign(ArrayList akeys)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string k in akeys)
            {
                string v = (string)parameters[k];
                if (null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + v + "&");
                }
            }

            sb.Append("key=" + this.getKey());
            string sign = MD5Util.GetMD5(sb.ToString(), TenpayConfig.InputCharset).ToLower();

            return getParameter("sign").ToLower().Equals(sign);
        }
    }
}
