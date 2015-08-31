using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;
using WitBird.SHTS.Areas.WeChatAuth.Utilities;

namespace WitBird.SHTS.Areas.WeChatAuth.MessageHandlers.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        private string appId = WebConfigurationManager.AppSettings["WeixinAppId"];
        private string appSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            WeixinContext.ExpireMinutes = 3;
        }

        public override void OnExecuting()
        {
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理事件请求。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "暂不支持直接回复消息访问活动在线网官方微信号，请从菜单选择相应操作。";

            return responseMessage;
        }

        protected void LogException(Exception ex)
        {
            try
            {
                #region LogException
                var logPath = HttpContext.Current.Server.MapPath("~/Error.txt");
                using (StreamWriter tw = new StreamWriter(logPath))
                {
                    tw.WriteLine("Time:" + DateTime.Now);
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.ToString());
                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.ToString());
                    }
                }
                #endregion
            }
            catch (Exception)
            {
                //TODO
            }
        }
    }
}
