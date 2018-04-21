using System;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP.Entities.Request;
using WitBird.SHTS.Areas.WeChatAuth.MessageHandlers.CustomMessageHandler;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.Entities;

namespace WitBird.SHTS.Areas.WeChatAuth.Controllers
{
    public class AuthController : Controller
    {
        public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];
        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];
        public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];
        public static readonly string AppSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];

        /// <summary>
        /// 微信后台验证地址（使用Get）
        /// </summary>
        [HttpGet]
        public ActionResult Index(string signature, string timestamp, string nonce, string echostr)
        {
            if (CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                //返回随机字符串则表示验证通过
                return Content(echostr);
            }
            else
            {
                return Content("failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, Token));
            }
        }

        /// <summary>
        /// 微信交互接口
        /// </summary>
        [HttpPost]
        public ActionResult Index(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Token;
            postModel.EncodingAESKey = EncodingAESKey;
            postModel.AppId = AppId;

            var maxRecordCount = 10;

            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, maxRecordCount);
            try
            {
                messageHandler.Execute();
                return new FixWeixinBugWeixinResult(messageHandler);
            }
            catch (Exception ex)
            {
                #region LogException
                var logPath = Server.MapPath("~/Error.txt");
                using (StreamWriter tw = new StreamWriter(logPath))
                {
                    tw.WriteLine("Time:" + DateTime.Now);
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.ToString());
                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.ToString());
                    }

                    tw.Flush();
                    tw.Close();
                }
                #endregion

                return Content("");
            }
        }

        public ActionResult CreateMenu()
        {
            GetMenuResult result = new GetMenuResult(new ButtonGroup());

            //初始化
            for (int i = 0; i < 3; i++)
            {
                var subButton = new SubButton();
                for (int j = 0; j < 5; j++)
                {
                    var singleButton = new SingleClickButton();
                    subButton.sub_button.Add(singleButton);
                }
            }

            return View(result);
        }

        [HttpPost]
        public ActionResult CreateMenu(GetMenuResultFull resultFull)
        {
            WxJsonResult result = null;
            try
            {
                var accessToken = AccessTokenContainer.TryGetAccessToken(AppId, AppSecret);
                var bg = CommonApi.GetMenuFromJsonResult(resultFull, new ButtonGroup()).menu;
                result = CommonApi.CreateMenu(accessToken, bg);
            }
            catch (Exception)
            {
                //TODO
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetToken(string appId, string appSecret)
        {
            try
            {
                if (!AccessTokenContainer.CheckRegistered(appId))
                {
                    AccessTokenContainer.Register(appId, appSecret);
                }
                var result = AccessTokenContainer.GetAccessTokenResult(appId);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { error = "执行过程发生错误！" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMenu()
        {
            var accessToken = AccessTokenContainer.TryGetAccessToken(AppId, AppSecret);
            var result = CommonApi.GetMenu(accessToken);
            if (result == null)
            {
                return Json(new { error = "菜单不存在或验证失败！" }, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
