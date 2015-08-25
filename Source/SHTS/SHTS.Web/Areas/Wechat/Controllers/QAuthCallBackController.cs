using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    /// <summary>
    /// 单独定义这个Controller得原因是绕过WeChatBaseController的ActionExcuting的权限检查，因为他本身就是检查中的一环。不然会造成死循环，一直在请求授权。
    /// </summary>
    public class QAuthCallBackController : Controller
    {
        //与WeChatBaseController中定义的应该一致
        public const string WeChatUserInfo = "WeChatUserInfo";
        public const string WeChatOpenIdCookieName = "WeChatOpenId";

        //
        // GET: /Wechat/QAuthCallBack/

        public ActionResult CallBack(string code, string state)
        {
            ActionResult result = null;

            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    result = Content("您拒绝了授权");
                }
                else
                {
                    var callBackUrl = state;
                    var appId = ConfigurationManager.AppSettings["WeixinAppId"];
                    var secret = ConfigurationManager.AppSettings["WeixinAppSecret"];

                    OAuthAccessTokenResult accessTokenResult = null;

                    //通过，用code换取access_token
                    try
                    {
                        accessTokenResult = OAuthApi.GetAccessToken(appId, secret, code);
                    }
                    catch (Exception ex)
                    {
                        LogService.LogWexin("获取AccessToken失败", ex.ToString());
                        result = Content("获取AccessToken失败");
                    }

                    //LogService.LogWexin("AccessToken请求状态", accessTokenResult.errcode.ToString());
                    if (accessTokenResult.errcode != ReturnCode.请求成功)
                    {
                        result = Content("获取AccessToken失败");
                    }
                    else
                    {
                        //更新用户Cookie
                        Response.Cookies.Add(new HttpCookie(WeChatOpenIdCookieName, accessTokenResult.openid));

                        //var openIdCookie = Request.Cookies[WeChatOpenIdCookieName];
                        //if (openIdCookie == null || string.IsNullOrEmpty(openIdCookie.Value))
                        //{
                        //    LogService.LogWexin("OpenID Cookie写入失败", "");
                        //}

                        //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
                        UserService userService = new UserService();
                        OAuthUserInfo userInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                        WeChatUser wechatUser = userService.GetWeChatUser(accessTokenResult.openid);

                        if (wechatUser == null)
                        {
                            userService.WeChatUserSubscribe(accessTokenResult.openid, false, true);
                            wechatUser = userService.GetWeChatUser(accessTokenResult.openid);
                        }
                        else
                        {
                            wechatUser.HasAuthorized = true;
                        }

                        if (wechatUser != null)
                        {
                            // Updates wechat user info
                            wechatUser.AccessToken = accessTokenResult.access_token;
                            wechatUser.AccessTokenExpired = false;
                            wechatUser.AccessTokenExpireTime = DateTime.Now.AddSeconds(accessTokenResult.expires_in);
                            wechatUser.City = userInfo.city;
                            wechatUser.County = userInfo.country;
                            wechatUser.NickName = userInfo.nickname;
                            wechatUser.Photo = userInfo.headimgurl;
                            wechatUser.Province = userInfo.province;
                            wechatUser.Sex = userInfo.sex;

                            if (userService.UpdateWeChatUser(wechatUser))
                            {
                                //Updates current wechat user
                                Session[WeChatUserInfo] = wechatUser;

                                //LogService.LogWexin("保存微信用户资料成功", "");
                            }
                            else
                            {
                                LogService.LogWexin("保存微信用户资料失败", wechatUser.OpenId);
                            }

                            // 跳转到用户一开始要进入的页面
                            result = Redirect(callBackUrl);
                            //LogService.LogWexin("跳转到用户一开始要进入的页面", callBackUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("QAuth获取用户信息失败", ex.ToString());
                result = Content("获取用户信息失败");
            }

            return result;
        }


        public ActionResult Test()
        {
            return View();
        }
    }
}
