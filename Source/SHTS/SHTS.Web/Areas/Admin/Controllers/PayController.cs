using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using WitBird.Com.Pay;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class PayController : AdminBaseController
    {
        //
        // GET: /Admin/Pay/
        PublicConfigService publicConfigService;
        public PayController()
        {
            publicConfigService = new PublicConfigService();
        }

        public ActionResult Index()
        {
            return View();
        }
        
        [Permission(EnumRole.Editer)]
        public ActionResult PayServiceConfig()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePaymentServiceConfig(string serviceName, bool isEnabled, string merchantId, string merchantKey)
        {
            string result = "更新失败";
            try
            {
                if (string.Equals(serviceName, PaymentService.ALIPAYSERVICE, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(serviceName, PaymentService.CHINABANKSERVICE, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(serviceName, PaymentService.TENPAYSERVICE, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (isEnabled &&
                        (string.IsNullOrWhiteSpace(merchantId) ||
                        string.IsNullOrWhiteSpace(merchantKey)))
                    {
                        throw new ArgumentException("商户ID和密钥不能为空");
                    }

                    string configValue = isEnabled.ToString() + "^" + merchantId + "^" + merchantKey;
                    PublicConfig config = new PublicConfig
                    {
                        ConfigName = serviceName,
                        ConfigValue = configValue,
                        CreatedTime = DateTime.Now,
                        LastUpdatedTime = DateTime.Now
                    };

                    if (publicConfigService.AddOrUpdateConfigValue(config))
                    {
                        Caching.Set(serviceName, configValue);
                        result = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("更新在线支付配置失败", ex.ToString());
                result = "更新失败——" + ex.Message;
            }

            return Content(result);
        }
    }
}
