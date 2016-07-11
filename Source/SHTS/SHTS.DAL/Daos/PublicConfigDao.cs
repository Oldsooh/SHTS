using System.Collections.Generic;
using System.Data.SqlClient;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL.Daos
{
    public class PublicConfigDao
    {
        #region SP const name

        private const string SP_GetConfigValueByID = "sp_GetConfigValueById";
        private const string SP_GetConfigValueByName = "sp_GetConfigValueByConfigName";
        private const string SP_AddOrUpdatePublicConfigValue = "sp_AddOrUpdatePublicConfigValue";
        private const string SP_AddPublicConfigValueIgnoreExists = "sp_AddPublicConfigValueIgnoreExists";
        private const string SP_DeletePublicConfigById = "sp_DeletePublicConfigById";

        #endregion SP const name

        /// <summary>
        /// Gets public config object by config id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public PublicConfig GetConfigValue(SqlConnection conn, int configId)
        {
            PublicConfig config = null;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigId", configId)
            };

            SqlDataReader reader = DBHelper.RunProcedure(conn, SP_GetConfigValueByID, sqlParameters);

            while (reader.Read())
            {
                config = ConvertToUserObject(reader);
            }

            if (reader != null)
            {
                reader.Close();
            }

            return config;
        }

        /// <summary>
        /// Gets public config object by config name.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public PublicConfig GetConfigValue(SqlConnection conn, string configName)
        {
            PublicConfig config = null;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigName", configName)
            };

            SqlDataReader reader = DBHelper.RunProcedure(conn, SP_GetConfigValueByName, sqlParameters);

            while (reader.Read())
            {
                config = ConvertToUserObject(reader);
            }

            if (reader != null)
            {
                reader.Close();
            }

            return config;
        }

        public List<PublicConfig> GetConfigValues(SqlConnection conn, string configName)
        {
            List<PublicConfig> configs = new List<PublicConfig>();
            PublicConfig config = null;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigName", configName)
            };

            SqlDataReader reader = DBHelper.RunProcedure(conn, SP_GetConfigValueByName, sqlParameters);

            while (reader.Read())
            {
                config = ConvertToUserObject(reader);
                configs.Add(config);
            }

            if (reader != null)
            {
                reader.Close();
            }

            return configs;
        }

        /// <summary>
        /// Adds new public config or updates exist config value.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool AddOrUpdateConfigValue(SqlConnection conn, PublicConfig config)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigId", config.ConfigId),
                new SqlParameter("@ConfigName", config.ConfigName),
                new SqlParameter("@ConfigValue", config.ConfigValue),
                new SqlParameter ("CreatedTime", config.CreatedTime),
                new SqlParameter("LastUpdatedTime", config.LastUpdatedTime)
            };

            return DBHelper.RunNonQueryProcedure(conn,
                SP_AddOrUpdatePublicConfigValue, parameters) > 0;
        }

        /// <summary>
        /// Adds new public config ignore exists.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool AddNewConfigIgnoreExists(SqlConnection conn, PublicConfig config)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigId", config.ConfigId),
                new SqlParameter("@ConfigName", config.ConfigName),
                new SqlParameter("@ConfigValue", config.ConfigValue),
                new SqlParameter ("CreatedTime", config.CreatedTime),
                new SqlParameter("LastUpdatedTime", config.LastUpdatedTime)
            };

            return DBHelper.RunNonQueryProcedure(conn,
                SP_AddPublicConfigValueIgnoreExists, parameters) > 0;
        }

        /// <summary>
        /// Deletes an exist config value by config id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public bool DeleteConfigValue(SqlConnection conn, int configId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigId", configId)
            };

            return DBHelper.RunNonQueryProcedure(conn,
                SP_DeletePublicConfigById, parameters) > 0;

        }

        private PublicConfig ConvertToUserObject(SqlDataReader reader)
        {
            PublicConfig config = new PublicConfig();

            config.ConfigId = reader["ConfigId"].DBToInt32();
            config.ConfigName = reader["ConfigName"].DBToString();
            config.ConfigValue = reader["ConfigValue"].DBToString();
            config.CreatedTime = reader["CreatedTime"].DBToDateTime().Value;
            config.LastUpdatedTime = reader["LastUpdatedTime"].DBToDateTime().Value;

            return config;
        }

    }
}
