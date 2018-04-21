using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace WitBird.Com.Pay.Tenpay
{
    /// <summary>
    /// ResponseHandler ��ժҪ˵����
    /// </summary>
    internal class ResponseHandler
    {
        /** ��Կ */
        private string key;

        /** Ӧ��Ĳ��� */
        protected Hashtable parameters;

        //��ȡ������֪ͨ���ݷ�ʽ�����в�����ȡ
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

        /** ��ȡ��Կ */
        public string getKey()
        { return key; }

        /** ������Կ */
        public void setKey(string key)
        { this.key = key; }

        /** ��ȡ����ֵ */
        public string getParameter(string parameter)
        {
            string s = (string)parameters[parameter];
            return (null == s) ? "" : s;
        }

        /** ���ò���ֵ */
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

        /** �Ƿ�Ƹ�ͨǩ��,������:����������a-z����,������ֵ�Ĳ������μ�ǩ���� 
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

        /** �Ƿ�Ƹ�ͨǩ��,������:����������a-z����,������ֵ�Ĳ������μ�ǩ���� 
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
