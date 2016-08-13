using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Witbird.SHTS.Model;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.AdvancedAPIs;
using Witbird.SHTS.Common;
using Senparc.Weixin.MP.Entities;

namespace Witbird.SHTS.Web.Areas.Wechat.Common
{
    public class WeChatClient
    {
        public static class App
        {
            /// <summary>
            /// 服务号AppId
            /// </summary>
            public static string AppId = ConfigurationManager.AppSettings["WeixinAppId"];
            /// <summary>
            /// 服务号AppSecrect
            /// </summary>
            public static string AppSecret = ConfigurationManager.AppSettings["WeixinAppSecret"];
        }

        public static class Constant
        {
            /// <summary>
            /// 微信主动交互时间即将超时
            /// </summary>
            public const string RequestTimeExceedMessage = @"亲爱的需求订阅用户，由于微信服务号消息限制，48小时内如无主动交互我们将无法向您推送您订阅的最新需求信息。为了保证您的正常使用，你可以:

1. 点击底部菜单
需求信息 -> 获取订阅

2. 点击底部菜单 
需求信息 -> 更新订阅";

            /// <summary>
            /// 当前没有可推荐的需求订阅信息
            /// </summary>
            public const string NoSubcribedDemansFound = @"活动在线客服MM暂时还没发布符合您的订阅规则的相关需求哦，请稍后再试。";

            /// <summary>
            /// 用户没有开启需求订阅，不能获取订阅内容
            /// </summary>
            public const string NotEnableSubscription = @"由于您没有开启需求订阅，暂时无法获取需求内容。请点击菜单 需求信息 -> 需求订阅设置 来开启吧！开启后，您就可以免费收到最新发布的需求内容哦！";

            /// <summary>
            /// 用户关注公众号时候的欢迎信息
            /// </summary>
            public const string WelcomeMessageWhenSubscribed = @"中国活动在线网(http://www.activity-line.com)是一个提供举办活动所需的资源网，与文艺演出、巡演、会议、展会、拓展训练及企业培训、婚礼及各类型赛事活动等相关，包含活动场地、演艺人员和工作人员、活动设备、媒体、翻译速记、摄像摄影、鲜花、礼品、餐饮等各类型资源，覆盖范围从一线城市、各省会城市到全国的各县级城市。
";
            
            /// <summary>
            /// 请先关注我们的链接
            /// </summary>
            public const string FollowUsUrl = "http://mp.weixin.qq.com/s?__biz=MzIzODAzMjg1Mg==&mid=406616045&idx=1&sn=0284c00c826b9faacc9fd51d61e90a31&scene=0&previewkey=hJ65r3CvPxZrCv2xPXuf8MNS9bJajjJKzz%2F0By7ITJA%3D#wechat_redirect";

            /// <summary>
            /// 关于我们的链接
            /// </summary>
            public const string AboutUsUrl = "http://mp.weixin.qq.com/s?__biz=MzIzODAzMjg1Mg==&mid=406616045&idx=1&sn=0284c00c826b9faacc9fd51d61e90a31&scene=0&previewkey=hJ65r3CvPxZrCv2xPXuf8MNS9bJajjJKzz%2F0By7ITJA%3D#wechat_redirect";

        }

        public static class Sender
        {
            /// <summary>
            /// 发送图文消息给指定用户，最多8条
            /// </summary>
            /// <param name="openId">用户OpenId</param>
            /// <param name="articles">图文消息，最多8条，否则发送失败</param>
            /// <returns></returns>
            public static bool SendArticles(string openId, List<Article> articles)
            {
                bool isSuccessFul = false;
                AccessTokenContainer.Register(App.AppId, App.AppSecret);

                try
                {
                    var wxResult = CustomApi.SendNews(App.AppId, openId, articles);
                    if (wxResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
                    {
                        isSuccessFul = true;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogWexin("Failed to send articles to user " + openId, ex.ToString());
                }

                return isSuccessFul;
            }

            /// <summary>
            /// 发送文字消息给指定用户
            /// </summary>
            /// <param name="openId">用户OpenId</param>
            /// <param name="message">消息内容</param>
            /// <returns></returns>
            public static bool SendText(string openId, string message)
            {
                bool isSuccessFul = false;
                AccessTokenContainer.Register(App.AppId, App.AppSecret);
                try
                {
                    var wxResult = CustomApi.SendText(App.AppId, openId, message);
                    if (wxResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
                    {
                        isSuccessFul = true;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogWexin("Failed to send text message to user " + openId, ex.ToString());
                }

                return isSuccessFul;
            }
        }
    }
}