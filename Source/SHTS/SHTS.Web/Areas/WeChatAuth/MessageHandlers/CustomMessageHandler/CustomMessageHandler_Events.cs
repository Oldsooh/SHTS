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

namespace WitBird.SHTS.Areas.WeChatAuth.MessageHandlers.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
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
            IResponseMessageBase reponseMessage = null;
            
            switch (requestMessage.EventKey)
            {
                //会员服务模块
                case "UserRegister"://会员注册
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        var url = "<a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/WeChat/Account/Register\">点击这里，立即注册</a>";
                        var content = "活动在线网微信服务号是活动在线网(www.activity-line.com)官方开发的一个提供举办活动所需各类资源的服务号。如需更好访问电脑、手机版及服务号，需注册账号成为会员。\r\n\r\n" + url;

                        strongResponseMessage.Content = content;
                        reponseMessage = strongResponseMessage;
                    }
                    break;
                case "UserLogin"://账号绑定
                    break;
                case "UserIdentity"://会员认证
                    break;

                //资源信息模块
                case "SpaceList"://活动场地
                    break;
                case "ActorList"://演艺人员
                    break;
                case "EquipmentList"://活动设备
                    break;
                case "OtherResourceList"://其他资源
                    break;

                //需求信息模块
                case "DemandList"://需求信息
                    break;
                case "NewDemand"://发布需求
                    break;
                case "TradeList"://交易中介
                    break;

                case "CLICK_EVENT":
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                        strongResponseMessage.Articles.Add(GetWelcomeInfo());
                        reponseMessage = strongResponseMessage;
                    }
                    break;
                default:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                        strongResponseMessage.Articles.Add(GetWelcomeInfo());
                        reponseMessage = strongResponseMessage;
                    }
                    break;
            }

            return reponseMessage;
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