using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Managers
{
    public class ConfigManager
    {
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
    }
}
