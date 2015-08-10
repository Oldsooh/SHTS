using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Common
{
    public class LogService
    {
        private static string logPath = ConfigurationManager.ConnectionStrings["logPath"].ConnectionString;
        private static string wexinLogPath = ConfigurationManager.ConnectionStrings["wexinLogPath"].ConnectionString;

        public static void Log(string name,string message)
        {
            if (!string.IsNullOrEmpty(logPath))
            {
                WitBirdLog loger = new WitBirdLog(logPath);
                loger.SaveLog(name, message);
            }
        }

        public static void LogWexin(string name, string message)
        {
            if (!string.IsNullOrEmpty(wexinLogPath))
            {
                WitBirdLog loger = new WitBirdLog(wexinLogPath);
                loger.SaveLog(name, message);
            }
        }
    }

    public class WitBirdLog
    {
        private static string _filePath;

        public WitBirdLog(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// 如果保存成功，则返回 “success”
        /// </summary>
        /// <param name="name">日志名称</param>
        /// <param name="message">日志内容</param>
        /// <returns>如果成功返回“success”，失败刚返回错误信息</returns>
        public string SaveLog(string name, string message)
        {
            string result = "failure";

            try
            {
                if (!string.IsNullOrEmpty(_filePath) && _filePath.Contains(".txt"))
                {
                    string folder = Directory.GetParent(_filePath).FullName;
                    //如果不存在path目录
                    if (!Directory.Exists(folder))
                    {
                        //那么就创建它
                        Directory.CreateDirectory(folder);
                    }
                    //如果txt文件不存在，则创建txt文件
                    if (!File.Exists(_filePath))
                    {
                        FileStream fstxt = new FileStream(_filePath, FileMode.Create);
                        StreamWriter swtxt = new StreamWriter(fstxt, Encoding.UTF8);
                        swtxt.Close();
                        fstxt.Close();
                    }
                    StreamWriter sw = new StreamWriter(_filePath, true);
                    try
                    {
                        //向文件中写入内容
                        sw.WriteLine(FormartLog(name, message));
                        result = "success";
                    }
                    catch (Exception ex2)
                    {
                        result = ex2.Message;
                    }
                    finally
                    {
                        //关闭当前文件写入流
                        sw.Close();
                        sw.Dispose();
                    }
                }
                else
                {
                    result = "路径不可用，必须包含 \\ 并以 .txt 结尾";
                }
            }
            catch (Exception ex1)
            {
                result = ex1.Message;
            }

            return result;
        }

        //格式化日志
        private static string FormartLog(string name, string message)
        {
            string info = DateTime.Now.ToString() + " -- [ " + name + " ]\r\n"
                             + "----------------------------------------------------------------------------------------------------------------\r\n"
                             + message
                             + "\r\n\r\n";
            return info;
        }
    }
}
