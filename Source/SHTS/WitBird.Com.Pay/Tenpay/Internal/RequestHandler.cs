using System;
using System.Collections;
using System.Text;

namespace WitBird.Com.Pay.Tenpay
{
	/// <summary>
	/// RequestHandler ��ժҪ˵����
	/// </summary>
	internal class RequestHandler
	{
		public RequestHandler()
		{
			parameters = new Hashtable();
			this.setGateUrl("https://www.tenpay.com/cgi-bin/v1.0/service_gate.cgi");
		}

		/** ����url��ַ */
		private string gateUrl;
		
		/** ��Կ */
		//private string key;
		
		/** ����Ĳ��� */
		protected Hashtable parameters;

		/** ��ʼ��������*/
		public virtual void init() 
		{
			//nothing to do
		}

		/** ��ȡ��ڵ�ַ,����������ֵ */
		public String getGateUrl() 
		{
			return gateUrl;
		}

		/** ������ڵ�ַ,����������ֵ */
		public void setGateUrl(String gateUrl) 
		{
			this.gateUrl = gateUrl;
		}

		/** ��ȡ������������URL  @return String */
		public virtual string getRequestURL()
		{
			this.createSign();

			StringBuilder sb = new StringBuilder();
			ArrayList akeys=new ArrayList(parameters.Keys); 
			akeys.Sort();
			foreach(string k in akeys)
			{
				string v = (string)parameters[k];
				if(null != v && "key".CompareTo(k) != 0) 
				{
					sb.Append(k + "=" + TenpayUtil.UrlEncode(v, TenpayConfig.InputCharset.Trim()) + "&");
				}
			}

			//ȥ�����һ��&
			if(sb.Length > 0)
			{
				sb.Remove(sb.Length-1, 1);
			}
							
			return this.getGateUrl() + "?" + sb.ToString();
		}

		/**
		* ����md5ժҪ,������:����������a-z����,������ֵ�Ĳ������μ�ǩ����
		*/
		public void createSign() 
		{
			StringBuilder sb = new StringBuilder();

			ArrayList akeys=new ArrayList(parameters.Keys); 
			akeys.Sort();

			foreach(string k in akeys)
			{
				string v = (string)parameters[k];
				if(null != v && "".CompareTo(v) != 0
					&& "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0) 
				{
					sb.Append(k + "=" + v + "&");
				}
			}

			sb.Append("key=" + TenpayConfig.TenpayKey);
			string sign = MD5Util.GetMD5(sb.ToString(), TenpayConfig.InputCharset.Trim().ToLower());
		
			this.setParameter("sign", sign);	
		}

		/** ��ȡ����ֵ */
		public string getParameter(string parameter) 
		{
			string s = (string)parameters[parameter];
			return (null == s) ? "" : s;
		}

		/** ���ò���ֵ */
		public void setParameter(string parameter,string parameterValue) 
		{
			if(parameter != null && parameter != "")
			{
				if(parameters.Contains(parameter))
				{
					parameters.Remove(parameter);
				}
	
				parameters.Add(parameter,parameterValue);		
			}
		}

		public Hashtable getAllParameters()
		{
			return this.parameters;
		}
	}
}
