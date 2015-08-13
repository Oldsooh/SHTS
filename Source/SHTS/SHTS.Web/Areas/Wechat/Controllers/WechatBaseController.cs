using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Configuration;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Controllers;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class WechatBaseController : BaseController
    {
        //与QAuthCallController中定义的应该一致
        public const string WeChatUserInfo = "WeChatUserInfo";
        public const string WeChatOpenIdCookieName = "WeChatOpenId";

        /// <summary>
        /// 请先关注我们的链接
        /// </summary>
        public const string AttentionUsUrl = "http://mp.weixin.qq.com/s?__biz=MzIzODAzMjg1Mg==&mid=210371076&idx=1&sn=206d0b4a6698e386f5960527a83a1320#rd";

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
                var wechatOpenIdCookie = filterContext.RequestContext.HttpContext.Request.Cookies[WeChatOpenIdCookieName];

                //用户还未授权或Cookie被清空, 重新授权。
                if (wechatOpenIdCookie == null || string.IsNullOrEmpty(wechatOpenIdCookie.Value))
                {
                    //LogService.Log("用户OpenId Cookie为空，需要授权", "");
                    // 授权回调页面
                    var redirectUrl = GetUrl("/wechat/QAuthCallBack/CallBack");
                    // 授权回调成功后跳转到用户一开始想访问的页面
                    var callBackUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
                    var appId = ConfigurationManager.AppSettings["WeixinAppId"];
                    var authUrl = OAuthApi.GetAuthorizeUrl(appId, redirectUrl, callBackUrl, OAuthScope.snsapi_userinfo);

                    filterContext.Result = new RedirectResult(authUrl);
                }
                else
                {
                    var userService = new UserService();

                    var wechatUser = userService.GetWeChatUser(wechatOpenIdCookie.Value);

                    //用户还未关注，提示用户关注我们先。
                    if (wechatUser == null || !wechatUser.HasSubscribed.HasValue || !wechatUser.HasSubscribed.Value)
                    {
                        filterContext.Result = new RedirectResult(AttentionUsUrl);
                    }
                    // 用户还未授权，先授权
                    else if (!wechatUser.HasAuthorized.HasValue || !wechatUser.HasAuthorized.Value)
                    {
                        // 授权回调页面
                        var redirectUrl = GetUrl("/wechat/QAuthCallBack/CallBack");
                        // 授权回调成功后跳转到用户一开始想访问的页面
                        var callBackUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
                        var appId = ConfigurationManager.AppSettings["WeixinAppId"];
                        var authUrl = OAuthApi.GetAuthorizeUrl(appId, redirectUrl, callBackUrl, OAuthScope.snsapi_userinfo);

                        filterContext.Result = new RedirectResult(authUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("获取用户授权页面出现错误", ex.ToString());
            }
        }
    }
}
