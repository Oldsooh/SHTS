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

namespace WitBird.SHTS.Areas.WeChatAuth.MessageHandlers.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
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
                case "CLICK_EVENT":
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                        strongResponseMessage.Articles.Add(new Article
                        {
                            Title = "活动在线微信版",
                            Description = "活动在线微信版",
                            PicUrl = "",
                            Url = ""
                        });
                        reponseMessage = strongResponseMessage;
                    }
                    break;
                default:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        strongResponseMessage.Content = "您点击了按钮，EventKey：" + requestMessage.EventKey;
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
            }
            catch (Exception ex)
            {
                LogException(ex);
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
            }
            catch (Exception ex)
            {
                LogException(ex);
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
            }
            catch (Exception ex)
            {
                LogException(ex);
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