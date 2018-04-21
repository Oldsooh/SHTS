using System.Collections.Generic;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Managers
{
    public class PublicConfigManager
    {
        PublicConfigService configService;

        public PublicConfigManager()
        {
            configService = new PublicConfigService();
        }

        /// <summary>
        /// Gets public config value by config id.
        /// </summary>
        /// <param name="configId">Config id.</param>
        /// <returns>Returns config value.</returns>
        public PublicConfig GetConfigValue(int configId)
        {
            return configService.GetConfigValue(configId);
        }

        /// <summary>
        /// Gets public config value by config name.
        /// </summary>
        /// <param name="configName">Config name.</param>
        /// <returns>Returns config value.</returns>
        public PublicConfig GetConfigValue(string configName)
        {
            return configService.GetConfigValue(configName);
        }

        /// <summary>
        /// Gets public config value by config name.
        /// </summary>
        /// <param name="configName">Config name.</param>
        /// <returns>Returns config value.</returns>
        public List<PublicConfig> GetConfigValues(string configName)
        {
            return configService.GetConfigValues(configName);
        }

        /// <summary>
        /// Adds new config or updates exist config.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool AddOrUpdateConfigValue(PublicConfig config)
        {
            return configService.AddOrUpdateConfigValue(config);
        }

        public bool AddConfigValueIgnoreExists(PublicConfig config)
        {
            return configService.AddConfigValueIgnoreExists(config);
        }

        /// <summary>
        /// Deletes config by config id.
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public bool DeleteConfig(int configId)
        {
            return configService.DeleteConfigValue(configId);
        }
    }
}
