using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;

namespace Witbird.SHTS.Web.Content.upload
{
    /// <summary>
    /// uploadimage 的摘要说明
    /// </summary>
    public class uploadvideo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/javascript";

            //存放图片的根目录 
            string folderName = "/uploadfiles/quotevideo/";
            string message = string.Empty;
            string outputFileName = string.Empty;
            var outputFileType = string.Empty;

            //定义错误消息
            try
            {
                //接受上传文件
                HttpPostedFile postFile = context.Request.Files["filedata"];
                if (postFile == null && context.Request.Files.Count>0)
                {
                    postFile = context.Request.Files[0];
                }

                LogService.Log("上传.......--:", context.Request.Files.Keys[0]);

                if (postFile != null)
                {
                    DateTime time = DateTime.Now;
                    //获取上传目录 转换为物理路径

                    var timePath = time.Year + "/" + time.ToString("MM") + "/" + time.ToString("dd") + "/";

                    string uploadPath = context.Server.MapPath("~" + folderName + timePath);
                    //文件名
                    string fileName = time.ToString("yyyyMMddHHmmssfff");
                    //后缀名称
                    string filetrype = System.IO.Path.GetExtension(postFile.FileName);
                    //获取文件大小
                    long contentLength = postFile.ContentLength;
                    //文件不能大于20M
                    if (contentLength <= 1024 * 1024 * 20)
                    {
                        //如果不存在path目录
                        if (!Directory.Exists(uploadPath))
                        {
                            //那么就创建它
                            Directory.CreateDirectory(uploadPath);
                        }
                        //保存文件的物理路径
                        string oldFile = uploadPath + fileName + filetrype;
                        try
                        {
                            //保存文件
                            postFile.SaveAs(oldFile);
                            outputFileName = folderName + timePath + fileName + filetrype;
                            outputFileType = filetrype.Replace(".", string.Empty);
                        }
                        catch(Exception ex)
                        {
                            message = "视频上传失败";
                            LogService.Log("上传失败--" + postFile.FileName, ex.ToString());
                        }

                    }
                    else
                    {
                        message = "视频大小不能超过20MB";
                    }
                }
                else
                {
                    message = "请选择视频文件";
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                LogService.Log("上传失败", e.ToString());
            }
            var data = new
            {
               IsSuccessful = !string.IsNullOrEmpty(message),
               FileName = outputFileName,
               FileType = outputFileType,
               ErrorMessage = message
            };
            context.Response.Write(data.ToJson());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}