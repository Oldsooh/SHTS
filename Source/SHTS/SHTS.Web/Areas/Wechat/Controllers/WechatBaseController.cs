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
        public const string AttentionUsUrl = "http://mp.weixin.qq.com/s?__biz=MzIzODAzMjg1Mg==&mid=210615414&idx=1&sn=b70bdf52770352541897d47f416e11d8#rd";

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
        /// 购买需求联系方式需要的价钱
        /// </summary>
        public decimal BuyDemandFee
        {
            get
            {
                decimal amount = 1m;//默认购买需要花费1元钱

                try
                {
                    if (!decimal.TryParse(ConfigurationManager.AppSettings["BuyDemandFee"], out amount))
                    {
                        amount = 1m;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogWexin("微信购买需求联系方式金额配置出错,请检查Web.config中<BuyDemandFee>配置", ex.ToString());
                    amount = 1m;
                }

                return amount;
            }
        }

        /// <summary>
        /// 微信权限检测
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //用于绕过权限检测，方便电脑版测试
            WeChatUser wechatUser = new UserService().GetWeChatUser(92);
            User user = new UserService().GetUserById(92);

            CurrentWeChatUser = wechatUser;
            CurrentUser = user;

            #region 微信权限检测
            //try
            //{
            //    var wechatOpenIdCookie = filterContext.RequestContext.HttpContext.Request.Cookies[WeChatOpenIdCookieName];

            //    //用户还未授权或Cookie被清空, 重新授权。
            //    if (wechatOpenIdCookie == null || string.IsNullOrEmpty(wechatOpenIdCookie.Value))
            //    {
            //        //LogService.Log("用户OpenId Cookie为空，需要授权", "");
            //        // 授权回调页面
            //        var redirectUrl = GetUrl("/wechat/QAuthCallBack/CallBack");
            //        // 授权回调成功后跳转到用户一开始想访问的页面
            //        var callBackUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
            //        var appId = ConfigurationManager.AppSettings["WeixinAppId"];
            //        var authUrl = OAuthApi.GetAuthorizeUrl(appId, redirectUrl, callBackUrl, OAuthScope.snsapi_userinfo);

            //        filterContext.Result = new RedirectResult(authUrl);
            //    }
            //    else
            //    {
            //        var userService = new UserService();

            //        var wechatUser = userService.GetWeChatUser(wechatOpenIdCookie.Value);

            //        //用户还未关注，提示用户关注我们先。
            //        if (wechatUser == null || !wechatUser.HasSubscribed.HasValue || !wechatUser.HasSubscribed.Value)
            //        {
            //            filterContext.Result = new RedirectResult(AttentionUsUrl);
            //        }
            //        // 用户还未授权，先授权
            //        else if (!wechatUser.HasAuthorized.HasValue || !wechatUser.HasAuthorized.Value)
            //        {
            //            // 授权回调页面
            //            var redirectUrl = GetUrl("/wechat/QAuthCallBack/CallBack");
            //            // 授权回调成功后跳转到用户一开始想访问的页面
            //            var callBackUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
            //            var appId = ConfigurationManager.AppSettings["WeixinAppId"];
            //            var authUrl = OAuthApi.GetAuthorizeUrl(appId, redirectUrl, callBackUrl, OAuthScope.snsapi_userinfo);

            //            filterContext.Result = new RedirectResult(authUrl);
            //        }
            //        else
            //        {
            //            if (wechatUser.UserId.HasValue)
            //            {
            //                var user = userService.GetUserById(wechatUser.UserId.Value);

            //                CurrentUser = user;
            //                wechatUser.IsUserLoggedIn = wechatUser.UserId.HasValue && IsUserLogin;
            //                wechatUser.IsUserIdentified = wechatUser.UserId.HasValue && IsIdentified;
            //                wechatUser.IsUserVip = wechatUser.UserId.HasValue && IsVip;
            //            }

            //            CurrentWeChatUser = wechatUser;

            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    LogService.LogWexin("获取用户授权页面出现错误", ex.ToString());
            //}

            #endregion 微信权限检测
        }
    }
}
