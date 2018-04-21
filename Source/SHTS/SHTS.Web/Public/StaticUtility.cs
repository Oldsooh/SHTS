using System;
using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using WitBird.Com.SMS;

namespace Witbird.SHTS.Web.Public
{
    public static class StaticUtility
    {
        private static Config config;
        private static EmailMessageUtil emailManager;

        /// <summary>
        /// 全局通用网站信息
        /// </summary>
        public static Config Config
        {
            get
            {
                if (config == null)
                {
                    config = ConfigManager.GetConfig();
                }
                if (config == null)
                {
                    config = new Config();
                    config.Name = "网站名称";
                    config.Title = "网站标题，请检查数据库是否正常.";
                }
                return config;
            }
        }
        public static void UpdateConfig()
        {
            config = ConfigManager.GetConfig();
        }

        public static EmailMessageUtil EmailManager
        {
            get
            {
                if (emailManager == null)
                {
                    MailConfig mailConfig = ConfigManager.GetMailConfig();
                    emailManager = new EmailMessageUtil(mailConfig.EmailServer, mailConfig.MailAccount, mailConfig.MailAccountName,
                        mailConfig.MailAccountPassword, mailConfig.EmailServerPort, mailConfig.EnableSSL, mailConfig.EnableAuthentication);
                }

                return emailManager;
            }
        }

        public static void UpdateEmailConfig()
        {
            MailConfig mailConfig = ConfigManager.GetMailConfig();
            emailManager = new EmailMessageUtil(mailConfig.EmailServer, mailConfig.MailAccount, mailConfig.MailAccountName,
                mailConfig.MailAccountPassword, mailConfig.EmailServerPort, mailConfig.EnableSSL, mailConfig.EnableAuthentication);
        }

        public static bool IsSpecialCityIdMatched(string sourceCityId, string targetCityId)
        {
            bool isMatched = false;

            if (string.IsNullOrEmpty(sourceCityId) || string.IsNullOrEmpty(targetCityId))
            {
                return isMatched;
            }

            if ((targetCityId.Equals("beijing", StringComparison.CurrentCultureIgnoreCase) ||
                targetCityId.Equals("shanghai", StringComparison.CurrentCultureIgnoreCase) ||
                targetCityId.Equals("chongqing", StringComparison.CurrentCultureIgnoreCase) ||
                targetCityId.Equals("tianjin", StringComparison.CurrentCultureIgnoreCase)) &&
                sourceCityId.StartsWith(targetCityId))
            {
                isMatched = true;
            }

            return isMatched;
        }

        public static bool IsSpecialCityNameMatched(string sourceCityName, string targetCityName)
        {
            bool isMatched = false;

            if (string.IsNullOrEmpty(sourceCityName) || string.IsNullOrEmpty(targetCityName))
            {
                return isMatched;
            }

            if ((targetCityName.Equals("北京", StringComparison.CurrentCultureIgnoreCase) ||
                targetCityName.Equals("上海", StringComparison.CurrentCultureIgnoreCase) ||
                targetCityName.Equals("重庆", StringComparison.CurrentCultureIgnoreCase) ||
                targetCityName.Equals("天津", StringComparison.CurrentCultureIgnoreCase)) &&
                sourceCityName.StartsWith(targetCityName))
            {
                isMatched = true;
            }

            return isMatched;
        }

        private static List<City> _allCities;
        /// <summary>
        /// 全局静态城市，包括省、市、区
        /// </summary>
        public static List<City> AllCities
        {
            get
            {
                if (_allCities == null)
                {
                    CityService cityService = new CityService();
                    _allCities = cityService.GetCities(true);
                }
                return _allCities;
            }
        }
        public static void UpdateCities()
        {
            CityService cityService = new CityService();
            _allCities = cityService.GetCities(true);
        }

        /// <summary>
        /// 获取 省份 或 城市 或 城区 的名称
        /// </summary>
        /// <param name="id">省、市、区Id</param>
        /// <returns></returns>
        public static string GetCityNameById(string id)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(id) && AllCities != null && AllCities.Count > 0)
            {
                foreach (var item in AllCities)
                {
                    if (item.Id == id)
                    {
                        result = item.Name;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 同时获取省份和城市的名称
        /// </summary>
        /// <param name="provinceId">省份Id</param>
        /// <param name="cityId">城市Id</param>
        /// <returns></returns>
        public static string GetProvinceAndCityNameById(string provinceId, string cityId)
        {
            return GetCityNameById(provinceId) + GetCityNameById(cityId);
        }

        public static string GetProvinceAndCityNameById(string provinceId, string cityId, string areaId)
        {
            return GetCityNameById(provinceId) + GetCityNameById(cityId) + GetCityNameById(areaId);
        }

        #region 三级联动

        /// <summary>
        /// 一级
        /// </summary>
        /// <returns></returns>
        public static List<City> GetProvice()
        {
            return AllCities.Where(v => v.IsActive && v.EntityType == 1).OrderBy(item => item.Sort).ToList();
        }

        /// <summary>
        /// 二级
        /// </summary>
        /// <returns></returns>
        public static List<City> GetCity(string provinceId)
        {
            if (string.IsNullOrEmpty(provinceId))
            {
                return new List<City>();
            }
            else
            {

                return AllCities.Where(v => v.IsActive && v.EntityType == 2 && v.ParentId != null && v.ParentId.Equals(provinceId))
                    .OrderBy(item => item.Sort).ToList();
            }
        }

        /// <summary>
        /// 三级
        /// </summary>
        /// <returns></returns>
        public static List<City> GetArea(string cityId)
        {
            return AllCities.Where(v => v.IsActive && v.EntityType == 3 && v.ParentId != null && v.ParentId.Equals(cityId))
                .OrderBy(item => item.Sort).ToList();
        }

        public static string GetLocationName(string locationId)
        {
            string result = "";

            if (!string.IsNullOrWhiteSpace(locationId))
            {
                var ids = locationId.Split(new char[] { '_' });
                // provinceId_cityId_areaId
                if (ids.Length == 3)
                {
                    string provinceName = GetCityNameById(ids[0]);
                    string cityName = GetCityNameById(ids[1]);
                    string areaName = GetCityNameById(ids[2]);

                    result = provinceName.DefaultIfNullOrEmpty() + "/" + cityName.DefaultIfNullOrEmpty() + '/' + areaName.DefaultIfNullOrEmpty();
                }
            }

            return result;
        }

        private static string DefaultIfNullOrEmpty(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                value = "---";
            }

            return value;
        }
        #endregion

    }
}