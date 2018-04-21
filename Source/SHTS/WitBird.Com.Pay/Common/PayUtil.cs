using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace WitBird.Com.Pay
{
    /// <summary>
    /// Online pay utility class
    /// </summary>
    public class PayUtil
    {
        /// <summary>
        /// Parses request result when there is un-known messy code.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static NameValueCollection DecodeRequest(Stream inputStream)
        {
            StreamReader sr = new StreamReader(inputStream, Encoding.GetEncoding(936));
            String query = sr.ReadToEnd();
            return HttpUtility.ParseQueryString(query, Encoding.GetEncoding(936));
        }
    }
}
