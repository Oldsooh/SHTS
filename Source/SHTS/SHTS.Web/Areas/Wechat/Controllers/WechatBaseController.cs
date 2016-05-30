﻿using Senparc.Weixin.MP;
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
        public const string FollowUsUrl = "http://mp.weixin.qq.com/s?__biz=MzIzODAzMjg1Mg==&mid=406616045&idx=1&sn=0284c00c826b9faacc9fd51d61e90a31&scene=0&previewkey=hJ65r3CvPxZrCv2xPXuf8MNS9bJajjJKzz%2F0By7ITJA%3D#wechat_redirect";
        // User service
        UserService userService = new UserService();

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

            //用于绕过权限检测，方便电脑测试
            //WeChatUser wechatUser = new UserService().GetWeChatUser(3);
            //User user = new UserService().GetUserById(3);

            //CurrentWeChatUser = wechatUser;
            //CurrentUser = user;
            //wechatUser.IsUserLoggedIn = IsUserLogin;
            //wechatUser.IsUserIdentified = IsIdentified;
            //wechatUser.IsUserVip = IsVip;

            #region 微信权限检测
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
                    var wechatUser = userService.GetWeChatUser(wechatOpenIdCookie.Value);

                    // 取消强制关注逻辑
                    //用户还未关注，提示用户关注我们先。
                    //if (wechatUser == null || !wechatUser.HasSubscribed.HasValue || !wechatUser.HasSubscribed.Value)
                    //{
                    //    filterContext.Result = new RedirectResult(FollowUsUrl);
                    //}
                    //else 

                    // 用户还未授权，先授权
                    if (wechatUser == null || !wechatUser.HasAuthorized.HasValue || !wechatUser.HasAuthorized.Value)
                    {
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

            }
            catch (Exception ex)
            {
                LogService.LogWexin("获取用户授权页面出现错误", ex.ToString());
            }

            #endregion 微信权限检测
        }
    }
}
