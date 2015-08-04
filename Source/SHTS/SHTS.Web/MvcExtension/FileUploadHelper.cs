using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Witbird.SHTS.Common;
using Witbird.SHTS.Web.Content.upload;

namespace Witbird.SHTS.Web.MvcExtension
{
    public class FileUploadHelper
    {
        const int width = 300;
        const int height = 200;

        /// <summary>
        /// 保存图片，错误返回Msg
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileidntity"></param>
        /// <returns></returns>
        public static string SaveFile(HttpContextBase context,string fileidntity,out string fileName)
        {
            //存放图片的根目录 
            const string folderName = "/uploadfiles/user/";
            const string mode = "HW";//生成缩略图的方式:HW(指定高宽缩放),W(指定宽，高按比例),H(指定高，宽按比例),Cut(指定高宽裁减)

            bool action = true;
            string big = "big.jpg";
            string small = "small.jpg";
            string message = string.Empty;
            fileName = folderName;
            //定义错误消息
            try
            {
                //接受上传文件
                HttpPostedFileBase postFile = context.Request.Files[fileidntity];
                if (postFile != null)
                {
                    DateTime time = DateTime.Now;
                    //获取上传目录 转换为物理路径
                    string webpath = folderName + time.Year + "/" + time.ToString("MM") + "/" +
                                     time.ToString("dd") + "/";
                    string uploadPath = context.Server.MapPath("~" + webpath);
                    //文件名
                    fileName = time.ToString("yyyyMMddHHmmssfff");
                    //后缀名称
                    string filetrype = System.IO.Path.GetExtension(postFile.FileName);
                    //获取文件大小
                    long contentLength = postFile.ContentLength;
                    //文件不能大于2M
                    if (contentLength <= 1024 * 2048)
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
                            //删除原图
                            if (File.Exists(oldFile))
                            {
                                File.Delete(oldFile);
                            }
                            //保存文件
                            postFile.SaveAs(oldFile);
                            fileName = webpath + fileName + filetrype;
                        }
                        catch
                        {
                            message = "上传失败";
                        }

                    }
                    else
                    {
                        message = "图片大小不能超过2MB";
                    }
                }
                else
                {
                    message = "请选择文件";
                }
            }
            catch (Exception e)
            {
                LogService.Log("上传图片", e.StackTrace);
                message = "出错了！";
            }
            return message;
        }
    }
}