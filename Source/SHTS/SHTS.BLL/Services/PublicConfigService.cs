using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    public class PublicConfigService
    {
        PublicConfigDao configDao;

        public PublicConfigService()
        {
            configDao = new PublicConfigDao();
        }

        /// <summary>
        /// Gets public config value by config id.
        /// </summary>
        /// <param name="configId">Config id.</param>
        /// <returns>Returns config value.</returns>
        public PublicConfig GetConfigValue(int configId)
        {
            PublicConfig config = new PublicConfig();
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                config = configDao.GetConfigValue(conn, configId);
            }
            catch (Exception e)
            {
                LogService.Log("查询Config失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return config;
        }

        /// <summary>
        /// Gets public config value by config name.
        /// </summary>
        /// <param name="configName">Config name.</param>
        /// <returns>Returns config value.</returns>
        public PublicConfig GetConfigValue(string configName)
        {
            PublicConfig config = new PublicConfig();
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                config = configDao.GetConfigValue(conn, configName);

                if (config == null)
                {
                    config = new PublicConfig() { ConfigName = configName, ConfigValue = string.Empty, CreatedTime = DateTime.Now, LastUpdatedTime = DateTime.Now };
                    configDao.AddOrUpdateConfigValue(conn, config);

                    // Retrieves config with id.
                    config = configDao.GetConfigValue(conn, configName);
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询Config失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return config;
        }

        /// Gets public config value by config name.
        /// </summary>
        /// <param name="configName">Config name.</param>
        /// <returns>Returns config value.</returns>
        public List<PublicConfig> GetConfigValues(string configName)
        {
            List<PublicConfig> configs = new List<PublicConfig>();
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                configs = configDao.GetConfigValues(conn, configName);
            }
            catch (Exception e)
            {
                LogService.Log("查询Config失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return configs;
        }

        /// <summary>
        /// Adds new config value or updates exist config value.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool AddOrUpdateConfigValue(PublicConfig config)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                config.CheckNullObject("参数");
                config.ConfigValue.CheckEmptyString("参数值");

                config.CreatedTime = DateTime.Now;
                config.LastUpdatedTime = DateTime.Now;

                conn.Open();
                result = configDao.AddOrUpdateConfigValue(conn, config);
            }
            catch (Exception e)
            {
                LogService.Log("添加或更新Config失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public bool AddConfigValueIgnoreExists(PublicConfig config)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                config.CheckNullObject("参数");
                config.ConfigValue.CheckEmptyString("参数值");

                config.CreatedTime = DateTime.Now;
                config.LastUpdatedTime = DateTime.Now;

                conn.Open();
                result = configDao.AddNewConfigIgnoreExists(conn, config);
            }
            catch (Exception e)
            {
                LogService.Log("添加Config失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Deletes an exist config value.
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public bool DeleteConfigValue(int configId)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                conn.Open();
                result = configDao.DeleteConfigValue(conn, configId);
            }
            catch (Exception e)
            {
                LogService.Log("删除Config失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

    }
}
