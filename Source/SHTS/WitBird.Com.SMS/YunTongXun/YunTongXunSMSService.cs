using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WitBird.Com.SMS.YunTongXun
{
    /// <summary>
    /// 云通信实现短信发送
    /// </summary>
    public class YunTongXunSMSService:ISendShortMessageService
    {
        private const string STatusCode = "statusCode";

        /// <summary>
        /// <see cref="ISendShortMessageService.SendShortMessage"/>
        /// </summary>
        public SMSResponse SendShortMessage(ShortMessage shortmessage)
        {
            SMSResponse response = new SMSResponse();

            CCPRestSDK.CCPRestSDK api = new CCPRestSDK.CCPRestSDK();
            //ip格式如下，不带https://
            bool isInit = api.init(SMSAccountInfo.Instance.SendAdress,
                SMSAccountInfo.Instance.CustomizedInfos[Constants.Port]);
            api.setAccount(SMSAccountInfo.Instance.AccountSID,
                SMSAccountInfo.Instance.AuthToken);
            api.setAppId(SMSAccountInfo.Instance.CustomizedInfos[Constants.APPID]);
            if (isInit)
            {
                var paras = shortmessage.Parameters == null ? 
                    new string[1] { shortmessage.Content } : shortmessage.Parameters;
                Dictionary<string, object> retData =
                    api.SendTemplateSMS(shortmessage.ToPhoneNumber,
                    SMSAccountInfo.Instance.CustomizedInfos[Constants.TemplateId], paras);
                if (string.Equals(retData[STatusCode].ToString(), "000000"))
                {
                   response.statusCode = "200";
                }
                else
                {
                    response.statusCode = "403";
                }
                response.ResponseData = getDictionaryData(retData);
            }
            else
            {
                response.statusCode = "103";
                response.ResponseData = "初始化失败";
            }
            return response;
        }

        private string getDictionaryData(Dictionary<string, object> data)
        {
            string ret = null;
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value != null && item.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    ret += item.Key.ToString() + "={";
                    ret += getDictionaryData((Dictionary<string, object>)item.Value);
                    ret += "};";
                }
                else
                {
                    ret += item.Key.ToString() + "=" + (item.Value == null ? "null" : item.Value.ToString()) + ";";
                }
            }
            return ret;
        }
    }
}
