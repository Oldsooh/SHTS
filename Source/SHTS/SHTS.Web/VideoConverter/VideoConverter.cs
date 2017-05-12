using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Witbird.SHTS.Common;

namespace Witbird.SHTS.Web
{
    public class VideoConverter
    {
        /// <summary>
        /// {0}: source filename, {1}: target filename
        /// </summary>
        private const string ArgumentFormat = "-i {0} -y {1} ";

        public static bool Convert(string ffmpegToolFileName, string sourceFileName, string targetFileName)
        {
            bool isSuccessful = false;

            if (string.IsNullOrEmpty(sourceFileName) ||
                string.IsNullOrEmpty(targetFileName) ||
                string.IsNullOrEmpty(ffmpegToolFileName) ||
                !File.Exists(sourceFileName) ||
                !File.Exists(ffmpegToolFileName))
            {
                isSuccessful = false;
            }
            else
            {
                try
                {

                    using (Process _process = new Process())
                    {
                        _process.StartInfo.FileName = ffmpegToolFileName;
                        _process.StartInfo.Arguments = string.Format(ArgumentFormat, sourceFileName, targetFileName);
                        _process.StartInfo.UseShellExecute = false;
                        _process.StartInfo.RedirectStandardError = true;
                        _process.StartInfo.CreateNoWindow = false;
                        _process.ErrorDataReceived += new DataReceivedEventHandler(DoErrorDataReceived);

                        _process.Start();
                        _process.BeginErrorReadLine();
                        _process.WaitForExit();
                    }

                    isSuccessful = true;
                }
                catch (Exception ex)
                {
                    isSuccessful = false;
                    LogService.LogWexin("Convert Video Failed", ex.ToString());
                }
            }

            return isSuccessful;
        }

        static void DoErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                //LogService.LogWexin("视频转换日志", e.Data);
            }
        }
    }
}