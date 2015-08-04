using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Witbird.SHTS.Common.Html
{
    public class HtmlUtil
    {
        /// <summary>
        /// 除去html元素中标记
        /// </summary>
        /// <param name="htmlStream">html文档</</param>
        /// <returns>无html的文档</returns>
        public static string RemoveHTMLTags(string htmlStream)
        {
            if (htmlStream == null)
            {
                throw new Exception("Your input html stream is null!");
            }
            /*
             * 最好把所有的特殊HTML标记都找出来，然后把与其相对应的Unicode字符一起影射到Hash表内，最后一起都替换掉
             */
            //先单独测试,成功后,再把所有模式合并
            //注:这两个必须单独处理
            //去掉嵌套了HTML标记的JavaScript:(<script)[\\s\\S]*(</script>)
            //去掉css标记:(<style)[\\s\\S]*(</style>)
            //去掉css标记:\\..*\\{[\\s\\S]*\\}
            htmlStream = Regex.Replace(htmlStream, "(<script)[\\s\\S]*?(</script>)|(<style)[\\s\\S]*?(</style>)", " ", RegexOptions.IgnoreCase);
            //htmlStream = RemoveTag(htmlStream, "script");
            //htmlStream = RemoveTag(htmlStream, "style");
            //去掉普通HTML标记:<[^>]+>
            //替换空格:&nbsp;|&amp;|&shy;|&#160;|&#173;
            htmlStream = Regex.Replace(htmlStream, "<[^>]+>|&nbsp;|&amp;|&shy;|&#160;|&#173;|&bull;|&lt;|&gt;", " ", RegexOptions.IgnoreCase);
            //htmlStream = RemoveTag(htmlStream);
            //替换左尖括号
            //htmlStream = Regex.Replace(htmlStream, "&lt;", "<");
            //替换右尖括号
            //htmlStream = Regex.Replace(htmlStream, "&gt;", ">");
            //替换空行
            //htmlStream = Regex.Replace(htmlStream, "[\n|\r|\t]", " ");//[\n|\r][\t*| *]*[\n|\r]
            htmlStream = Regex.Replace(htmlStream, "(\r\n[\r|\n|\t| ]*\r\n)|(\n[\r|\n|\t| ]*\n)", "\r\n");
            htmlStream = Regex.Replace(htmlStream, "[\t| ]{1,}", " ");
            return htmlStream.Trim();
        }

        /// <summary>
        /// 除去所有在html元素中标记
        /// </summary>
        /// <param name="strhtml">html文档</param>
        /// <returns>无html的文档</returns>
        public static string StripHtml(string strhtml)
        {
            string stroutput = strhtml;
            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            stroutput = regex.Replace(stroutput, "");
            return stroutput;
        }

        //使用MD5加密用户密码的方法，返回32位的密文字符串
        public static string GetMd5Str(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
            byte[] toData = md5.ComputeHash(fromData);
            string byteStr = null;
            for (int i = 0; i < toData.Length; i++)
            {
                byteStr += toData[i].ToString("x2");
            }
            return byteStr;
        }
    }
}
