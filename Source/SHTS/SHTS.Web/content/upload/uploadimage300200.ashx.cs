using System;
using System.IO;
using System.Web;

namespace Witbird.SHTS.Web.Content.upload
{
    /// <summary>
    /// uploadimage300200 的摘要说明
    /// </summary>
    public class uploadimage300200 : IHttpHandler
    {
        int width = 300;
        int height = 200;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            //存放图片的根目录 
            string folderName = "/uploadfiles/";
            string mode = "HW";//生成缩略图的方式:HW(指定高宽缩放),W(指定宽，高按比例),H(指定高，宽按比例),Cut(指定高宽裁减)

            bool action = true;
            string big = "big.jpg";
            string small = "small.jpg";
            string message = "ok";

            //定义错误消息
            try
            {
                //接受上传文件
                HttpPostedFile postFile = context.Request.Files["upImage"];
                if (postFile != null)
                {
                    DateTime time = DateTime.Now;
                    //获取上传目录 转换为物理路径
                    string uploadPath = context.Server.MapPath("~" + folderName + time.Year + "/" + time.ToString("MM") + "/" + time.ToString("dd") + "/");
                    //文件名
                    string fileName = time.ToString("yyyyMMddHHmmssfff");
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
                        string oldFile = uploadPath + fileName + "temp" + filetrype;
                        string bigFile = uploadPath + fileName + "big" + filetrype;
                        try
                        {
                            //保存文件
                            postFile.SaveAs(oldFile);
                            //保存缩略图的物理路径
                            string thumbnail = uploadPath + fileName + filetrype;
                            MakeThumbnail(oldFile, thumbnail, width, height, mode);
                            big = folderName + time.Year + "/" + time.ToString("MM") + "/" + time.ToString("dd") + "/" + fileName + filetrype;
                            small = folderName + time.Year + "/" + time.ToString("MM") + "/" + time.ToString("dd") + "/" + fileName + filetrype;
                            //删除原图
                            if (File.Exists(oldFile))
                            {
                                File.Delete(oldFile);
                            }
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
                message = e.Message;
            }

            string json = "{'action':'" + action.ToString() + "','big':'" + big + "','small':'" + small + "','message':'" + message + "'}";
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式:HW(指定高宽缩放),W(指定宽，高按比例),H(指定高，宽按比例),Cut(指定高宽裁减)</param>
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            //创建新图片（暂时和原图片一样）
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;//缩略图宽度
            int toheight = height;//缩略图高度

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            //查看生产类型
            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）
                    break;
                case "W"://指定宽，高按比例   
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量，低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);
            //在指定位置并且按指定大小绘制原图的制定部分
            g.DrawImage(originalImage,
                new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
    }
}