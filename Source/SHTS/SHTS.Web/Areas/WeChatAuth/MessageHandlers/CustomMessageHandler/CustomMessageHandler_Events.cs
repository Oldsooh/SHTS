using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using WitBird.SHTS.Areas.WeChatAuth.Utilities;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;

namespace WitBird.SHTS.Areas.WeChatAuth.MessageHandlers.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {

        private static string UserLoginUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/Account/Login\">点击这里，立即绑定</a>";
        private static string UserReLoginUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/Account/Login\">点击这里，重新绑定</a>";
        private static string UserRegisterUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/Account/Register\">点击这里，立即注册</a>";
        private static string UserIdentifyUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/User/Identify\">点击这里，立即认证</a>";

        UserService userService = new UserService();

        private Article GetWelcomeInfo()
        {
            return new Article()
            {
                Title = "活动在线微信版",
                Description = "活动在线微信版",
                PicUrl = "",
                Url = ""
            };
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase responseMessage = null;
            var openId = requestMessage.FromUserName;
            var content = "";

            switch (requestMessage.EventKey)
            {
                #region 会员注册
                case "UserRegister":
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        content = "活动在线网微信服务号是活动在线网(www.activity-line.com)官方开发的服务号。如需更好访问电脑、手机版及服务号，需注册账号成为会员。\r\n\r\n" 
                            + UserRegisterUrl;

                        strongResponseMessage.Content = content;
                        responseMessage = strongResponseMessage;
                    }
                    break;
                #endregion 

                #region 账号绑定
                case "UserLogin":
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        var hasUserLoggedIn = false;
                        WeChatUser wechatUser = null;
                        User user = null;

                        try
                        {
                            wechatUser = userService.GetWeChatUser(openId);

                            if (wechatUser != null)
                            {
                                if (wechatUser.UserId.HasValue)
                                {
                                    user = userService.GetUserById(wechatUser.UserId.Value);

                                    if (user != null)
                                    {
                                        hasUserLoggedIn = true;
                                    }
                                }
                            }
                            else
                            {
                                content = "连接获取失败，请重新尝试。";
                            }
                        }
                        catch(Exception ex)
                        {
                            LogService.LogWexin("微信用户绑定发生错误", ex.ToString());
                            content = "连接获取失败，请重新尝试。";
                        }

                        if (hasUserLoggedIn)
                        {
                            var userName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : (!string.IsNullOrEmpty(user.Email) ? user.Email : user.Cellphone);
                            content = "绑定活动在线网(www.activity-line.com)会员账号，微信登录即可同步PC端会员账号数据。您当前已绑定账号为：" 
                                + userName + "。是否需要更改绑定账号？\r\n\r\n" + UserReLoginUrl;
                        }
                        else if (string.IsNullOrEmpty(content))
                        {
                            content = "活动在线网微信服务号是活动在线网(www.activity-line.com)官方开发的服务号。如需更好访问电脑、手机版及服务号，请立即绑定您的会员账号。\r\n\r\n"
                                   + UserLoginUrl;
                        }
                        else
                        {
                            // do nothing.
                        }

                        strongResponseMessage.Content = content;
                        responseMessage = strongResponseMessage;
                    }

                    break;
                #endregion 

                #region 会员认证
                case "UserIdentity":
                    {
                        var hasUserIdentified = false;
                        WeChatUser wechatUser = null;
                        User user = null;

                        try
                        {
                            wechatUser = userService.GetWeChatUser(openId);

                            if (wechatUser != null)
                            {
                                if (wechatUser.UserId.HasValue)
                                {
                                    user = userService.GetUserById(wechatUser.UserId.Value);
                                    
                                    if(user != null 
                                        && user.Vip.HasValue
                                        && (user.Vip.Value == (int)VipState.Identified || user.Vip.Value == (int)VipState.VIP))
                                    {
                                        hasUserIdentified = true;
                                    }
                                }
                            }
                            else
                            {
                                content = "连接获取失败，请重新尝试。";
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.LogWexin("微信用户会员认证发生错误", ex.ToString());
                            content = "连接获取失败，请重新尝试。";
                        }

                        if (hasUserIdentified)
                        {
                            var newsMessage = CreateResponseMessage<ResponseMessageNews>();
                            var userName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : (!string.IsNullOrEmpty(user.Email) ? user.Email : user.Cellphone);

                            newsMessage.Articles.Add(new Article 
                            {
                                Description = "点击消息查看认证详情。",
                                PicUrl = user.IdentiyImg,
                                Title = "您的账号：" + userName + "已认证。",
                                Url = UserIdentifyUrl
                            });

                            responseMessage = newsMessage;
                        }
                        else if (user != null)
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            var userName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : (!string.IsNullOrEmpty(user.Email) ? user.Email : user.Cellphone);
                            content = "活动在线认证会员可以发布资源信息、需求信息及活动在线，并且能够查看所有资源信息的联系方式。您当前认证的账号为：" + userName + "。\r\n\r\n" + UserIdentifyUrl;

                            strongResponseMessage.Content = content;
                            responseMessage = strongResponseMessage;
                        }
                        else if (string.IsNullOrEmpty(content))
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            var userName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : (!string.IsNullOrEmpty(user.Email) ? user.Email : user.Cellphone);
                            content = "您当前还未绑定账号。" + userName + "。请先绑定活动在线会员账号，如还未注册，请点击会员注册。\r\n\r\n" + UserLoginUrl;

                            strongResponseMessage.Content = content;
                            responseMessage = strongResponseMessage;
                        }
                        else
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>(); 
                            strongResponseMessage.Content = content;
                            responseMessage = strongResponseMessage;
                        }

                    }

                    break;
                #endregion

                #region 活动场地
                case "SpaceList":
                    break;
                #endregion

                #region 演艺人员
                case "ActorList":
                    break;
                #endregion 

                #region 活动设备
                case "EquipmentList":
                    break;
                #endregion

                #region 其他资源
                case "OtherResourceList":
                    break;
                #endregion 
                
                #region 需求信息
                case "DemandList":
                    break;
                #endregion 

                #region 发布需求
                case "NewDemand":
                    break;
                #endregion

                #region 交易中介
                case "TradeList":
                    break;
                #endregion 

                #region 默认返回
                default:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                        strongResponseMessage.Articles.Add(GetWelcomeInfo());
                        responseMessage = strongResponseMessage;
                    }
                    break;
                #endregion
            }

            return responseMessage;
        }

        /// <summary>
        /// 微信自动发送过来的位置信息.
        /// </summary>
        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = string.Format("{0}:{1}", requestMessage.Latitude, requestMessage.Longitude);
            return responseMessage;
        }

        /// <summary>
        /// 通过扫描关注
        /// </summary>
        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            strongResponseMessage.Articles.Add(GetWelcomeInfo());
            try
            {
                // 关注事件，下边边注册微信用户
                UserService userService = new UserService();
                bool result = userService.WeChatUserSubscribe(requestMessage.FromUserName);
                if (!result)
                {
                    LogService.Log("微信关注注册用户失败", "WeChatID = " + requestMessage.FromUserName);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("微信关注注册用户失败, WeChatID = " + requestMessage.FromUserName, ex.ToString());
            }

            return strongResponseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            strongResponseMessage.Articles.Add(GetWelcomeInfo());
            try
            {
                // 关注事件，下边边注册微信用户
                UserService userService = new UserService();
                bool result = userService.WeChatUserSubscribe(requestMessage.FromUserName);
                if (!result)
                {
                    LogService.Log("微信关注注册用户失败", "WeChatID = " + requestMessage.FromUserName);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("微信关注注册用户失败, WeChatID = " + requestMessage.FromUserName, ex.ToString());
            }

            return strongResponseMessage;
        }

        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            try
            {
                // 取消关注事件，下边做些逻辑
                UserService userService = new UserService();
                userService.UnSubscribeWeChatUser(requestMessage.FromUserName);
            }
            catch (Exception ex)
            {
                LogService.Log("用户取消关注失败， WeChatID = " + requestMessage.FromUserName, ex.ToString());
            }
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出地理位置选择器";
            return responseMessage;
        }
    }
}