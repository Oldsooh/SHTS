using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Web.Areas.Wechat.Common;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class WechatBaseController : Witbird.SHTS.Web.Controllers.BaseController
    {
        /// <summary>
        /// 当前是否是用电脑测试，绕过微信授权检测方便电脑测试
        /// </summary>
        protected bool RunWeChatWebSiteOnPCEnvironment = ConfigurationManager.AppSettings["RunWeChatWebSiteOnPCEnvironment"].ToBoolean();

        //与QAuthCallController中定义的应该一致
        public const string WeChatUserInfo = "WeChatUserInfo";
        public const string WeChatOpenIdCookieName = "WeChatOpenId";

        // User service
        UserService userService = new UserService();

        /// <summary>
        /// 当前访问的WeChat用户信息
        /// </summary>
        public WeChatUser CurrentWeChatUser
        {
            get
            {
                return Session[WeChatUserInfo] as WeChatUser;
            }
            set
            {
                Session[WeChatUserInfo] = value;
            }
        }

        /// <summary>
        /// 当前WeChat用户绑定的PC端账号信息
        /// </summary>
        public override User CurrentUser
        {
            get
            {
                return base.CurrentUser;
            }
            set
            {
                base.CurrentUser = value;
            }
        }

        /// <summary>
        /// 在执行具体Action之前进行微信权限检测，保存wechat用户信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            SetDefaultCityToSession();

            try
            {
                // 当前请求是否来自微信授权结果
                bool isRequestFromWeChatAuth = false;

                if (RunWeChatWebSiteOnPCEnvironment)
                {
                    SetWeChatUserSessionForTestingUseOnly();
                }
                else
                {
                    // 用户同意授权返回的code, 用于换取access_token,如果code不为空，表示之前发起授权操作，优先处理授权结果
                    var code = filterContext.HttpContext.Request.QueryString["code"];
                    // state用于保存一些临时信息，暂时未使用
                    //var state = filterContext.HttpContext.Request.QueryString["state"];

                    isRequestFromWeChatAuth = !string.IsNullOrEmpty(code);
                    if (isRequestFromWeChatAuth)
                    {
                        if (!HandleWechatAuthResult(code))
                        {
                            filterContext.Result = GetWechatAuthFailedResult();
                        }
                        else
                        {
                            // 授权成功重定向到目标页面
                            var redirectUrl = GetOriginalUrlString(filterContext);
                            filterContext.Result = new RedirectResult(redirectUrl);
                        }
                    }
                    else
                    {
                        bool isWeChatAuthRequired = SetWeChatUserInfoWithValidation(filterContext);
                        if (isWeChatAuthRequired)
                        {
                            filterContext.Result = GetWechatAuthUrlString(filterContext);
                        }
                    }
                }

                //if (string.IsNullOrEmpty(CurrentCityId))
                //{
                //    filterContext.Result = new RedirectResult("/wechat/city/index?returnUrl=" + filterContext.HttpContext.Request.Url.OriginalString);
                //}
            }
            catch (Exception ex)
            {
                LogService.LogWexin("获取用户授权页面出现错误", ex.ToString());
                filterContext.Result = GetWechatAuthFailedResult();
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Response.Redirect("/wechat/error/error_503");
        }
        /// <summary>
        /// 检测微信用户授权信息，如果不符合则需要重新进行微信授权验证。
        /// </summary>
        /// <param name="filterContext"></param>
        private bool SetWeChatUserInfoWithValidation(ActionExecutingContext filterContext)
        {
            bool isWeChatAuthRequired = false;
            var wechatOpenIdCookie = filterContext.RequestContext.HttpContext.Request.Cookies[WeChatOpenIdCookieName];

            //用户还未授权或Cookie被清空, 重新授权。
            if (wechatOpenIdCookie == null || string.IsNullOrEmpty(wechatOpenIdCookie.Value))
            {
                isWeChatAuthRequired = true;
            }
            else
            {
                var wechatUser = userService.GetWeChatUser(wechatOpenIdCookie.Value);

                // 取消强制关注逻辑
                //用户还未关注，提示用户关注我们先。
                //if (wechatUser == null || !wechatUser.HasSubscribed.HasValue || !wechatUser.HasSubscribed.Value)
                //{
                //    filterContext.Result = new RedirectResult(FollowUsUrl);
                //}
                //else 

                // 未获得用户数据，重新授权
                if (wechatUser == null || !wechatUser.HasAuthorized.HasValue || !wechatUser.HasAuthorized.Value)
                {
                    isWeChatAuthRequired = true;
                }
                else
                {
                    //更新Session信息
                    if (wechatUser.UserId.HasValue)
                    {
                        var user = userService.GetUserById(wechatUser.UserId.Value);

                        if (user != null)
                        {
                            CurrentUser = user;
                            wechatUser.IsUserLoggedIn = IsUserLogin;
                            wechatUser.IsUserIdentified = IsIdentified;
                            wechatUser.IsUserVip = IsVip;
                        }
                    }

                    CurrentWeChatUser = wechatUser;
                }
            }

            return isWeChatAuthRequired;
        }

        /// <summary>
        /// 返回微信授权页面
        /// </summary>
        /// <param name="filterContext"></param>
        private ActionResult GetWechatAuthUrlString(ActionExecutingContext filterContext)
        {
            #region Old code
            // 授权回调页面
            //var redirectUrl = GetUrl("/wechat/QAuthCallBack/CallBack");
            // 授权回调成功后跳转到用户一开始想访问的页面
            //var callBackUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
            #endregion

            var redirectUrl = GetOriginalUrlString(filterContext);
            var state = string.Empty; //保存一些临时值，暂未使用
            var authUrl = OAuthApi.GetAuthorizeUrl(WeChatClient.App.AppId, redirectUrl, state, OAuthScope.snsapi_userinfo);

            return new RedirectResult(authUrl);
        }

        /// <summary>
        /// 返回授权失败处理结果
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWechatAuthFailedResult()
        {
            return new RedirectResult("/wechat/error/error_503");
        }

        /// <summary>
        /// 处理微信授权结果，更新用户信息
        /// </summary>
        /// <param name="filterContext"></param>
        private bool HandleWechatAuthResult(string code)
        {
            bool isSuccessful = true;
            try
            {
                OAuthAccessTokenResult accessTokenResult = null;

                //通过，用code换取access_token
                accessTokenResult = OAuthApi.GetAccessToken(WeChatClient.App.AppId, WeChatClient.App.AppSecret, code);

                //LogService.LogWexin("AccessToken请求状态", accessTokenResult.errcode.ToString());
                if (accessTokenResult.errcode == ReturnCode.请求成功)
                {
                    //更新用户Open id Cookie
                    var openIdCookie = new HttpCookie(WeChatOpenIdCookieName, accessTokenResult.openid);
                    openIdCookie.Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Add(openIdCookie);

                    //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
                    UserService userService = new UserService();
                    OAuthUserInfo userInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                    WeChatUser wechatUser = userService.GetWeChatUser(accessTokenResult.openid);

                    // 用户第一次访问，添加用户信息
                    if (wechatUser == null)
                    {
                        userService.WeChatUserSubscribe(accessTokenResult.openid, false, true);
                        wechatUser = userService.GetWeChatUser(accessTokenResult.openid);
                    }
                    else
                    {
                        wechatUser.HasAuthorized = true;
                    }

                    // 更新用户详细信息
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
                            isSuccessful = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("QAuth获取用户信息发生异常", ex.ToString());
                isSuccessful = false;
            }

            return isSuccessful;
        }

        /// <summary>
        /// 删除微信跳转过来的链接中的code和state参数
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private string GetOriginalUrlString(ActionExecutingContext filterContext)
        {
            var originalUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
            var parameters = filterContext.HttpContext.Request.QueryString;
            var paraDic = new Dictionary<string, string>();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var key in parameters.AllKeys)
                {
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(parameters[key]) && !paraDic.ContainsKey(key))
                    {
                        paraDic.Add(key, parameters[key]);
                    }
                }
            }

            if (paraDic.ContainsKey("code"))
            {
                paraDic.Remove("code");
            }

            if (paraDic.ContainsKey("state"))
            {
                paraDic.Remove("state");
            }

            var queryString = string.Empty;

            int i = 0;
            foreach (var item in paraDic)
            {
                if (i == 0)
                {
                    queryString += item.Key + "=" + item.Value;
                }
                else
                {
                    queryString += "&" + item.Key + "=" + item.Value;
                }
                i++;
            }


            var queryStringIndex = originalUrl.IndexOf("?");
            var urlWithoutQueryString = originalUrl;

            if (queryStringIndex != -1)
            {
                urlWithoutQueryString = originalUrl.Substring(0, queryStringIndex);
            }

            return urlWithoutQueryString + "?" + queryString;
        }

        /// <summary>
        /// 设置默认微信用户session信息来绕过微信授权检测，仅用于电脑测试。
        /// </summary>
        private void SetWeChatUserSessionForTestingUseOnly()
        {
            WeChatUser wechatUser = new UserService().GetWeChatUserByWeChatUserId(112816);
            if (wechatUser.IsNotNull())
            {
                CurrentWeChatUser = wechatUser;
                if (wechatUser.UserId.HasValue)
                {

                    User user = new UserService().GetUserById(wechatUser.UserId.Value);
                    if (user.IsNotNull())
                    {
                        CurrentUser = user;

                        wechatUser.IsUserLoggedIn = IsUserLogin;
                        wechatUser.IsUserIdentified = IsIdentified;
                        wechatUser.IsUserVip = IsVip;
                    }
                }
            }
        }
    }
}
