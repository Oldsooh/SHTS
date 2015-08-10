using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Controllers;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class WechatBaseController : BaseController
    {
        public const string WeChatUserInfo = "WeChatUserInfo";
        public WeChatUser CurrentWeChatUser
        {
            get
            {
                return Session[WeChatUserInfo] as WeChatUser;
            }
        }
        public override bool RequireLogin()
        {
            if (CurrentWeChatUser == null)
            {
                Response.Redirect("/wechat/account/login");
                return true;
            }
            return false;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            try
            {
                bool needAuthorize = true;

                if (CurrentWeChatUser != null && !string.IsNullOrEmpty(CurrentWeChatUser.OpenId))
                {
                    needAuthorize = false;
                }

                if (needAuthorize)
                {
                    var redirectUrl = GetUrl("/wechat/wechatbase/QAuthCallBack");
                    var callBackUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
                    var appId = ConfigurationManager.AppSettings["WeixinAppId"];
                    var authUrl = OAuthApi.GetAuthorizeUrl(appId, redirectUrl, callBackUrl, OAuthScope.snsapi_userinfo);

                    filterContext.Result = Redirect(authUrl);
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("获取用户授权页面出现错误", ex.ToString());
            }
        }

        public ActionResult QAuthCallBack(string code, string state)
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
                        result = Content("用户授权失败");
                    }

                    if (accessTokenResult.errcode != ReturnCode.请求成功)
                    {
                        result = Content("授权失败");
                    }
                    else
                    {
                        //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息

                        UserService userService = new UserService();
                        OAuthUserInfo userInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                        WeChatUser weChatUser = userService.GetWeChatUser(CurrentWeChatUser.Id);

                        // Updates wechat user info
                        weChatUser.AccessToken = accessTokenResult.access_token;
                        weChatUser.AccessTokenExpired = false;
                        weChatUser.AccessTokenExpireTime = DateTime.Now.AddSeconds(accessTokenResult.expires_in);
                        weChatUser.City = userInfo.city;
                        weChatUser.County = userInfo.country;
                        weChatUser.NickName = userInfo.nickname;
                        weChatUser.OpenId = userInfo.openid;
                        weChatUser.Photo = userInfo.headimgurl;
                        weChatUser.Province = userInfo.province;
                        weChatUser.Sex = userInfo.sex;

                        if (userService.UpdateWeChatUser(weChatUser))
                        {
                            //Updates current wechat user
                            Session[WeChatUserInfo] = weChatUser;
                        }
                        else
                        {
                            LogService.LogWexin("保存微信用户资料失败", "");
                        }

                        // 跳转到用户一开始要进入的页面
                        result = Redirect(callBackUrl);
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.LogWexin("QAuth获取用户信息失败", ex.ToString());
                result = Content("获取用户信息失败");
            }

            return result;
        }
    }
}
