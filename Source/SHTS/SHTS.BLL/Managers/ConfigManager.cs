using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Managers
{
    public class ConfigManager
    {
        static PublicConfigService publicConfigService = new PublicConfigService();

        public static Config GetConfig()
        {
            Config result = null;
            try
            {
                ConfigRepository configRepository = new ConfigRepository();
                result = configRepository.FindOne(c => c.Id == 1);
            }
            catch (Exception e)
            {
                LogService.Log("查询配置信息失败", e.ToString());
            }
            return result;
        }

        public static bool EditConfig(Config config)
        {
            bool result = false;
            try
            {
                if (config != null)
                {
                    ConfigRepository configRepository = new ConfigRepository();
                    result = configRepository.UpdateEntitySave(config);
                }
            }
            catch (Exception e)
            {
                LogService.Log("编辑配置信息失败", e.ToString());
            }
            return result;
        }

        public static MailConfig GetMailConfig()
        {
            MailConfig mailConfig = new MailConfig();
            
            try
            {
                mailConfig.EmailServer = publicConfigService.GetConfigValue(Constant.EmailServerHostName).ConfigValue;
                mailConfig.EmailServerPort = publicConfigService.GetConfigValue(Constant.EmailServerPort).ConfigValue.ToInt32();
                mailConfig.EnableAuthentication = publicConfigService.GetConfigValue(Constant.EnableAuthentication).ConfigValue.ToBoolean();
                mailConfig.EnableSSL = publicConfigService.GetConfigValue(Constant.EnableSSL).ConfigValue.ToBoolean();
                mailConfig.MailAccount = publicConfigService.GetConfigValue(Constant.MailAccount).ConfigValue;
                mailConfig.MailAccountName = publicConfigService.GetConfigValue(Constant.MailAccountName).ConfigValue;
                mailConfig.MailAccountPassword = publicConfigService.GetConfigValue(Constant.MailAccountPassword).ConfigValue;
            }
            catch(Exception ex)
            {
                LogService.Log("获取邮箱设置出错", ex.ToString());
            }

            return mailConfig;
        }

        public static bool UpdateMailConfig(MailConfig mailConfig)
        {
            bool isSuccessFul = true;

            try
            {
                if (mailConfig != null)
                {
                    PublicConfig config = null;
                    DateTime transactionDateTime = DateTime.Now;

                    config = publicConfigService.GetConfigValue(Constant.EmailServerHostName);

                    if (!config.ConfigValue.Equals(mailConfig.EmailServer))
                    {
                        config.ConfigValue = mailConfig.EmailServer;
                        config.LastUpdatedTime = transactionDateTime;
                        isSuccessFul = isSuccessFul && publicConfigService.AddOrUpdateConfigValue(config);
                    }

                    config = publicConfigService.GetConfigValue(Constant.EmailServerPort);
                    if (!config.ConfigValue.Equals(mailConfig.EmailServerPort))
                    {
                        config.ConfigValue = mailConfig.EmailServerPort.ToString();
                        config.LastUpdatedTime = transactionDateTime;
                        isSuccessFul = isSuccessFul && publicConfigService.AddOrUpdateConfigValue(config);
                    }

                    config = publicConfigService.GetConfigValue(Constant.EnableAuthentication);
                    if (!config.ConfigValue.Equals(mailConfig.EnableAuthentication))
                    {
                        config.ConfigValue = mailConfig.EnableAuthentication.ToString();
                        config.LastUpdatedTime = transactionDateTime;
                        isSuccessFul = isSuccessFul && publicConfigService.AddOrUpdateConfigValue(config);
                    }

                    config = publicConfigService.GetConfigValue(Constant.EnableSSL);
                    if (!config.ConfigValue.Equals(mailConfig.EnableSSL))
                    {
                        config.ConfigValue = mailConfig.EnableSSL.ToString();
                        config.LastUpdatedTime = transactionDateTime;
                        isSuccessFul = isSuccessFul && publicConfigService.AddOrUpdateConfigValue(config);
                    }

                    config = publicConfigService.GetConfigValue(Constant.MailAccount);
                    if (!config.ConfigValue.Equals(mailConfig.MailAccount))
                    {
                        config.ConfigValue = mailConfig.MailAccount;
                        config.LastUpdatedTime = transactionDateTime;
                        isSuccessFul = isSuccessFul && publicConfigService.AddOrUpdateConfigValue(config);
                    }

                    config = publicConfigService.GetConfigValue(Constant.MailAccountName);
                    if (!config.ConfigValue.Equals(mailConfig.MailAccountName))
                    {
                        config.ConfigValue = mailConfig.MailAccountName;
                        config.LastUpdatedTime = transactionDateTime;
                        isSuccessFul = isSuccessFul && publicConfigService.AddOrUpdateConfigValue(config);
                    }

                    config = publicConfigService.GetConfigValue(Constant.MailAccountPassword);
                    if (!config.ConfigValue.Equals(mailConfig.MailAccountPassword))
                    {
                        config.ConfigValue = mailConfig.MailAccountPassword;
                        config.LastUpdatedTime = transactionDateTime;
                        isSuccessFul = isSuccessFul && publicConfigService.AddOrUpdateConfigValue(config);
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.Log("更新邮箱配置出错", ex.ToString());
                isSuccessFul = false;
            }

            return isSuccessFul;
        }
    }
}
