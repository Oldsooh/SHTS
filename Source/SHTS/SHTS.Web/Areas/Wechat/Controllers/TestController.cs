using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System.Web.Mvc;
using Witbird.SHTS.Web.Areas.Wechat.Common;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class TestController : WechatBaseController
    {
        public ActionResult SendTemplateMessage(string openId, string templateId, string first, string key1, string key2)
        {
            var status = "ok";
            var message = "";

            if (string.IsNullOrWhiteSpace(openId) || string.IsNullOrWhiteSpace(templateId) || string.IsNullOrWhiteSpace(first))
            {
                status = "fail";
                message = "参数缺失";
            }
            else
            {
                var templateMsgData = new { first = new TemplateDataItem(first), keyword1 = new TemplateDataItem(key1), keyword2 = new TemplateDataItem(key2) };
                var result = WeChatClient.Sender.SendTemplateMessage(openId, templateId, templateMsgData);
                if (result.IsSuccessful)
                {
                    status = "ok";
                    message = "success";
                }
                else
                {
                    status = "fail";
                    message = $"Send failed. Error: {result.ErrorMessage}";
                }

            }

            var data = new { Status = status, Message = message };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
