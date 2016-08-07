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
using Witbird.SHTS.Web.Areas.Wechat.Common;

namespace WitBird.SHTS.Areas.WeChatAuth.MessageHandlers.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {

        public static string UserLoginUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/Account/Login\">点击这里，立即绑定</a>";
        public static string UserReLoginUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/Account/Login\">点击这里，重新绑定</a>";
        public static string UserRegisterUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/Account/Register\">点击这里，立即注册</a>";
        public static string UserIdentifyUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/User/Identify\">点击这里，立即认证</a>";
        public static string UserIdentifiedUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/User/Identify";

        public static string PlaceListUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/resource/spacelist";
        public static string ActorListUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/resource/actorlist";
        public static string EquipmentListUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/resource/equipmentlist";
        public static string OtherResourceUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/resource/otherlist";

        public static string DemandListUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/demand/index";
        public static string DemandAddUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/demand/add";
        public static string TradeListUrl = Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/trade/index";

        public static string BannerImgUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/content/images/banner.jpg";

        /// <summary>
        /// 0: 关键字， 1: 关键字
        /// </summary>
        public static string SearchResultUrl = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain +
            "/wechat/index/search?keyWords={0}&page=1\">点击查看关键字\"{1}\"的的搜索结果。</a>";

        UserService userService = new UserService();

        private Article GetWelcomeInfo()
        {
            return new Article()
            {
                Title = "欢迎您关注中国活动在线网",
                Description = WeChatClient.Constant.WelcomeMessageWhenSubscribed,
                PicUrl = BannerImgUrl,
                Url = WeChatClient.Constant.AboutUsUrl
            };
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase responseMessage = null;
            var openId = requestMessage.FromUserName;
            var content = "";
            WeChatUser wechatUser = null;

            var textResponseMessage = CreateResponseMessage<ResponseMessageText>();
            var newsResponseMessage = CreateResponseMessage<ResponseMessageNews>();

            switch (requestMessage.EventKey)
            {
                #region 会员注册
                case "UserRegister":
                    {
                        content = string.Empty;

                        User user = null;

                        try
                        {
                            wechatUser = userService.GetWeChatUser(openId);

                            if (wechatUser != null)
                            {
                                if (wechatUser.UserId.HasValue)
                                {
                                    user = userService.GetUserById(wechatUser.UserId.Value);
                                }
                            }
                            else
                            {
                                content = "连接获取失败，请重新尝试。";
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.LogWexin("微信用户绑定发生错误", ex.ToString());
                            content = "连接获取失败，请重新尝试。";
                        }

                        if (string.IsNullOrEmpty(content))
                        {
                            if (user != null)
                            {
                                var userName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : (!string.IsNullOrEmpty(user.Email) ? user.Email : user.Cellphone);
                                content = "您当前已绑定会员账号：" + userName + "，无需重复注册。如要重新注册新的会员账号，请点击如下链接。\r\n\r\n"
                                    + UserRegisterUrl;
                            }
                            else
                            {
                                content = "活动在线网微信服务号是活动在线网(www.activity-line.com)官方开发的服务号。如需更好访问电脑、手机版及服务号，需注册账号成为会员。\r\n\r\n"
                                    + UserRegisterUrl;
                            }
                        }

                        textResponseMessage.Content = content;
                        responseMessage = textResponseMessage;
                    }
                    break;
                #endregion

                #region 账号绑定
                case "UserLogin":
                    {
                        content = string.Empty;

                        var hasUserLoggedIn = false;
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
                        catch (Exception ex)
                        {
                            LogService.LogWexin("微信用户绑定发生错误", ex.ToString());
                            content = "连接获取失败，请重新尝试。";
                        }

                        if (string.IsNullOrEmpty(content))
                        {
                            if (hasUserLoggedIn)
                            {
                                var userName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : (!string.IsNullOrEmpty(user.Email) ? user.Email : user.Cellphone);
                                content = "绑定活动在线网(www.activity-line.com)会员账号，微信登录即可同步PC端会员账号数据。您当前已绑定账号为："
                                    + userName + "。是否需要更改绑定账号？\r\n\r\n" + UserReLoginUrl;
                            }
                            else
                            {
                                content = "活动在线网微信服务号是活动在线网(www.activity-line.com)官方开发的服务号。如需更好访问电脑、手机版及服务号，请立即绑定您的会员账号。\r\n\r\n"
                                       + UserLoginUrl;
                            }
                        }

                        textResponseMessage.Content = content;
                        responseMessage = textResponseMessage;
                    }

                    break;
                #endregion

                #region 会员认证
                case "UserIdentity":
                    {
                        content = string.Empty;

                        var hasUserIdentified = false;
                        User user = null;

                        try
                        {
                            wechatUser = userService.GetWeChatUser(openId);

                            if (wechatUser != null)
                            {
                                if (wechatUser.UserId.HasValue)
                                {
                                    user = userService.GetUserById(wechatUser.UserId.Value);

                                    if (user != null
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
                            var userName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : (!string.IsNullOrEmpty(user.Email) ? user.Email : user.Cellphone);

                            newsResponseMessage.Articles.Add(new Article
                            {
                                Description = "点击消息查看认证详情。",
                                PicUrl = user.IdentiyImg,
                                Title = "您的账号：" + userName + "已认证。",
                                Url = UserIdentifiedUrl
                            });

                            responseMessage = newsResponseMessage;
                        }
                        else if (string.IsNullOrEmpty(content))
                        {
                            if (user != null)
                            {
                                var userName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : (!string.IsNullOrEmpty(user.Email) ? user.Email : user.Cellphone);
                                content = "活动在线认证会员可以发布资源信息、需求信息及活动在线，并且能够查看所有资源信息的联系方式。您当前认证的账号为：" + userName + "。\r\n\r\n" + UserIdentifyUrl;

                                textResponseMessage.Content = content;
                                responseMessage = textResponseMessage;
                            }
                            else
                            {
                                content = "您当前还未绑定活动在线账号, 无法完成会员认证。请先绑定活动在线会员账号。\r\n\r\n" + UserLoginUrl
                                    + "\r\n\r\n如还未注册，请点击如下链接注册成为活动在线会员。\r\n\r\n" + UserRegisterUrl;

                                textResponseMessage.Content = content;
                                responseMessage = textResponseMessage;
                            }
                        }
                        else
                        {
                            textResponseMessage.Content = content;
                            responseMessage = textResponseMessage;
                        }
                    }

                    break;
                #endregion

                #region 活动场地
                case "SpaceList":
                    {
                        content = string.Empty;

                        newsResponseMessage.Articles.Add(new Article
                        {
                            Description = "点击查看详情。",
                            PicUrl = BannerImgUrl,
                            Title = "活动场地信息",
                            Url = PlaceListUrl
                        });

                        responseMessage = newsResponseMessage;
                    }
                    break;
                #endregion

                #region 演艺人员
                case "ActorList":
                    {
                        content = string.Empty;

                        newsResponseMessage.Articles.Add(new Article
                        {
                            Description = "点击查看详情。",
                            PicUrl = BannerImgUrl,
                            Title = "演艺人员信息",
                            Url = ActorListUrl
                        });

                        responseMessage = newsResponseMessage;
                    }
                    break;
                #endregion

                #region 活动设备
                case "EquipmentList":
                    {
                        content = string.Empty;

                        newsResponseMessage.Articles.Add(new Article
                        {
                            Description = "点击查看详情。",
                            PicUrl = BannerImgUrl,
                            Title = "活动设备信息",
                            Url = EquipmentListUrl
                        });

                        responseMessage = newsResponseMessage;
                    }
                    break;
                #endregion

                #region 其他资源
                case "OtherResourceList":
                    {
                        content = string.Empty;

                        newsResponseMessage.Articles.Add(new Article
                        {
                            Description = "点击查看详情。",
                            PicUrl = BannerImgUrl,
                            Title = "其他资源信息",
                            Url = OtherResourceUrl
                        });

                        responseMessage = newsResponseMessage;
                    }
                    break;
                #endregion

                #region 需求信息
                case "DemandList":
                    {
                        content = string.Empty;

                        newsResponseMessage.Articles.Add(new Article
                        {
                            Description = "点击查看详情。",
                            PicUrl = BannerImgUrl,
                            Title = "需求信息",
                            Url = DemandListUrl
                        });

                        responseMessage = newsResponseMessage;
                    }
                    break;
                #endregion

                #region 发布需求
                case "NewDemand":
                    {
                        content = string.Empty;

                        newsResponseMessage.Articles.Add(new Article
                        {
                            Description = "点击进入发布需求页面。",
                            PicUrl = BannerImgUrl,
                            Title = "发布需求",
                            Url = DemandAddUrl
                        });

                        responseMessage = newsResponseMessage;
                    }
                    break;
                #endregion

                #region 交易中介
                case "TradeList":
                    {
                        content = string.Empty;

                        textResponseMessage.Content = "欲查看交易中介信息，请用电脑访问" + TradeListUrl;

                        responseMessage = textResponseMessage;
                    }
                    break;
                #endregion

                #region 更新交互时间
                case "UpdateRequestTime":
                    {
                        content = string.Empty;
                        textResponseMessage.Content = "更新成功。最新交互时间： " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt");
                        responseMessage = textResponseMessage;
                    }
                    break;
                #endregion

                #region 获取订阅内容
                case "GetSubscribedDemands":
                    Witbird.SHTS.Web.Areas.Wechat.Subscription.WorkingThread.Instance.SendSubscribedDemandManually(openId);
                    break;
                #endregion

                #region 默认返回
                default:
                    {
                        textResponseMessage.Content = "暂不支持直接回复消息访问活动在线网官方微信号，请从菜单选择相应操作。";
                        responseMessage = textResponseMessage;
                    }
                    break;
                #endregion
            }

            // Update request time.
            userService.UpdateWeChatUserLastRequestTimestamp(openId);

            return responseMessage;
        }

        /// <summary>
        /// 微信自动发送过来的位置信息.
        /// </summary>
        //public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        //{
        //    var responseMessage = CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = string.Format("{0}:{1}", requestMessage.Latitude, requestMessage.Longitude);
        //    return responseMessage;
        //}

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