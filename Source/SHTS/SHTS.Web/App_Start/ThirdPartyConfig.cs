using Witbird.SHTS.Common;
using WitBird.Com.SMS;
using Newtonsoft.Json;

namespace Witbird.SHTS.Web
{
    public class ThirdPartyConfig
    {

        /// <summary>
        /// 在这里初始化第三方工具的信息。
        /// </summary>
        public static void Config()
        {
            ConfigSMS();
        }

        /// <summary>
        /// 配置短消息
        /// </summary>
        private static void ConfigSMS()
        {
            FileConfigService fileConfigService = new FileConfigService();
            var SMSConfig = fileConfigService.GetConfig("SMS.json");
            if (!string.IsNullOrEmpty(SMSConfig))
            {
                SMSAccountInfo.Initialize(
                    JsonConvert.DeserializeObject<SMSAccountInfo>(SMSConfig));
            }
        }

        private static void InitializeOnlinePaymentService()
        {
            FileConfigService fileConfigService = new FileConfigService();
            var servicesConfig = fileConfigService.GetConfig("PaymentServices.json");
            if (!string.IsNullOrEmpty(servicesConfig))
            {

            }
        }
    }
}