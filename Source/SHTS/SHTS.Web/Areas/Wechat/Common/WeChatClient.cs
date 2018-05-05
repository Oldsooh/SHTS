using System;
using System.Collections.Generic;
using System.Configuration;
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
            public const string RequestTimeExceedMessage = @"如需平台向你发送业务信息，请点击底部菜单【业务订单】→【获取更新】";

            /// <summary>
            /// 当前没有可推荐的需求订阅信息
            /// </summary>
            public const string NoSubcribedDemansFound = @"活动在线客服MM暂时还没发布符合您的订阅规则的相关需求哦，请稍后再试。";

            /// <summary>
            /// 用户没有开启需求订阅，不能获取订阅内容
            /// </summary>
            public const string NotEnableSubscription = @"由于您没有开启需求订阅，暂时无法获取需求内容。请点击菜单【业务订单】→【订阅业务】来开启吧！开启后，您就可以免费收到最新发布的需求内容哦！";

            /// <summary>
            /// 用户关注公众号时候的欢迎信息
            /// </summary>
            public const string WelcomeMessageWhenSubscribed = @"中国活动在线网(http://www.huodongzaixian.com)是一个提供举办活动所需的资源网，与文艺演出、巡演、会议、展会、拓展训练及企业培训、婚礼及各类型赛事活动等相关，包含活动场地、演艺人员和工作人员、活动设备、媒体、翻译速记、摄像摄影、鲜花、礼品、餐饮等各类型资源，覆盖范围从一线城市、各省会城市到全国的各县级城市。
";
            
            /// <summary>
            /// 请先关注我们的链接
            /// </summary>
            public const string FollowUsUrl = "http://mp.weixin.qq.com/s?__biz=MzIzODAzMjg1Mg==&mid=406616045&idx=1&sn=0284c00c826b9faacc9fd51d61e90a31&scene=0&previewkey=hJ65r3CvPxZrCv2xPXuf8MNS9bJajjJKzz%2F0By7ITJA%3D#wechat_redirect";

            /// <summary>
            /// 关于我们的链接
            /// </summary>
            public const string AboutUsUrl = "http://mp.weixin.qq.com/s?__biz=MzIzODAzMjg1Mg==&mid=406616045&idx=1&sn=0284c00c826b9faacc9fd51d61e90a31&scene=0&previewkey=hJ65r3CvPxZrCv2xPXuf8MNS9bJajjJKzz%2F0By7ITJA%3D#wechat_redirect";

            public const string FollowUrlBeforeAccess = "http://mp.weixin.qq.com/s/XEWi6MFylTIs6xfM5lUIsA";
            // 使用自定义关注页面
            //public const string FollowUrlBeforeAccess = "/wechat/QAuthCallBack/subscriberequired";


            public class TemplateMessage
            {
                /*
                 *  模版IDgQSO61afq1-RXP659u9R-qszNKwiluurk2mIPCUokpo
                    开发者调用模版消息接口时需提供模版ID
                    标题  报价提醒
                    行业IT科技 - 互联网|电子商务
                    详细内容
                    {{first.DATA}}
                    交易编号：{{keyword1.DATA}}
                    报价日期：{{keyword2.DATA}}
                    {{remark.DATA}}
                    在发送时，需要将内容中的参数（{{.DATA}}内为参数）赋值替换为需要的信息
                    内容示例
                    你有一条报价提醒
                    交易编号：12312345
                    报价日期：2015-3-31 17:44:13
                    请及时查看！
                 */
                /// <summary>
                /// 报价提醒模板消息
                /// </summary>
                public const string QuoteRemind = "gQSO61afq1-RXP659u9R-qszNKwiluurk2mIPCUokpo";

                /*
                 *  模版ID:i3lCzajW9NA1qb34K5aEr0aGJPGEh-DvZ98EHPxq3_0
                    开发者调用模版消息接口时需提供模版ID
                    标题  需求消息提醒
                    行业商业服务 - 广告|会展
                    详细内容
                    {{first.DATA}}
                    需求类型：{{keyword1.DATA}}
                    需求内容：{{keyword2.DATA}}
                    联系人：{{keyword3.DATA}}
                    联系电话：{{keyword4.DATA}}
                    发布时间：{{keyword5.DATA}}
                    {{remark.DATA}}
                    在发送时，需要将内容中的参数（{{.DATA}}内为参数）赋值替换为需要的信息
                    内容示例
                    办会易提醒您，您收到一个需求咨询
                    需求类型：供应需求
                    需求内容：场地搭建-篷房搭建
                    联系人：刘先生
                    联系电话：17791214939
                    发布时间：2016年1月2日 17:58
                    请移步至个人中心查看详情，如有疑问，请拨打咨询热线4006-177-029。
                 */
                /// <summary>
                /// 需求消息提醒模板消息
                /// </summary>
                public const string DemandRemind = "i3lCzajW9NA1qb34K5aEr0aGJPGEh-DvZ98EHPxq3_0";
            }

        }

        public static class Sender
        {
            /// <summary>
            /// 发送图文消息给指定用户，最多8条
            /// </summary>
            /// <param name="openId">用户OpenId</param>
            /// <param name="articles">图文消息，最多8条，否则发送失败</param>
            /// <returns></returns>
            public static OperationResult SendArticles(string openId, List<Article> articles)
            {
                var result = new OperationResult();
                AccessTokenContainer.Register(App.AppId, App.AppSecret);

                try
                {
                    var wxResult = CustomApi.SendNews(App.AppId, openId, articles);
                    if (wxResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
                    {
                        result.IsSuccessful = true;
                    }
                    else
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = wxResult.errcode.ToString();
                        var paramsData = new { openId = openId, errorMessage = wxResult.errmsg };
                        LogService.LogWexin("Failed to send SendArticles message to user, parameters info: ", paramsData.ToString());
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = ex.ToString();
                    LogService.LogWexin("Failed to send articles to user " + openId, ex.ToString());
                }

                return result;
            }

            /// <summary>
            /// 发送文字消息给指定用户
            /// </summary>
            /// <param name="openId">用户OpenId</param>
            /// <param name="message">消息内容</param>
            /// <returns></returns>
            public static OperationResult SendText(string openId, string message)
            {
                var result = new OperationResult();
                try
                {
                    AccessTokenContainer.Register(App.AppId, App.AppSecret);
                    var wxResult = CustomApi.SendText(App.AppId, openId, message);
                    if (wxResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
                    {
                        result.IsSuccessful = true;
                    }
                    else
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = wxResult.errcode.ToString();
                        var paramsData = new { openId = openId, message = message, errorMessage = wxResult.errmsg };
                        LogService.LogWexin("Failed to send SendText message to user, parameters info: ", paramsData.ToString());
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = ex.ToString();
                    LogService.LogWexin("Failed to send text message to user " + openId, ex.ToString());
                }

                return result;
            }

            /// <summary>
            /// 发送模板消息给用户
            /// </summary>
            /// <param name="openId"></param>
            /// <param name="templateId"></param>
            /// <param name="topcolor"></param>
            /// <param name="url"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public static OperationResult SendTemplateMessage(string openId, string templateId, object data = null, string url = "")
            {
                var result = new OperationResult();

                try
                {
                    AccessTokenContainer.Register(App.AppId, App.AppSecret);
                    
                    var topcolor = "";
                    var sendResult = TemplateApi.SendTemplateMessage(App.AppId, openId, templateId, topcolor, url, data);
                    if (sendResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
                    {
                        result.IsSuccessful = true;
                    }
                    else
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = sendResult.errcode.ToString();
                        var paramsData = new { openId = openId, templateId = templateId, url = url, sendResult = sendResult.errmsg };
                        LogService.LogWexin("Failed to send template message to user, parameters info: ", paramsData.ToString());
                    }
                }
                catch(Exception ex)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = ex.ToString();
                    var paramsData = new { openId = openId, templateId = templateId, url = url, data = data};
                    LogService.LogWexin("Failed to send template message to user, parameters info: " + paramsData.ToString(), ex.ToString());
                }

                return result;
            }
        }
    }
}