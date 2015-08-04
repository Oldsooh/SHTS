using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Public
{
    public class StaticUtility
    {
        private static Config config;
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
                    if (item.EntityType != 1 && item.Id == id)
                    {
                        if (item.Id != "beijingshi" &&
                           item.Id != "shanghaishi" &&
                           item.Id != "tianjinshi" &&
                           item.Id != "chongqingshi")//如果是直辖市则不返回“直辖市”
                        {
                            result = item.Name;
                        }
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
            string result = string.Empty;

            if (!string.IsNullOrEmpty(provinceId))
            {
                if (AllCities != null && AllCities.Count > 0)
                {
                    foreach (var item in AllCities)
                    {
                        if (item.EntityType == 1 && item.Id == provinceId)
                        {
                            if (provinceId != "beijingshi" &&
                                provinceId != "shanghaishi" &&
                                provinceId != "tianjinshi" &&
                                provinceId != "chongqingshi")//如果是直辖市则不返回“直辖市”
                            {
                                result = item.Name;
                            }
                            break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(cityId))
            {
                if (AllCities != null && AllCities.Count > 0)
                {
                    foreach (var item in AllCities)
                    {
                        if (item.EntityType == 2 && item.Id == cityId)
                        {
                            result += item.Name;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public static string GetProvinceAndCityNameById(string provinceId, string cityId, string areaId)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(provinceId))
            {
                if (AllCities != null && AllCities.Count > 0)
                {
                    foreach (var item in AllCities)
                    {
                        if (item.EntityType == 1 && item.Id == provinceId)
                        {
                            if (provinceId != "beijingshi" &&
                                provinceId != "shanghaishi" &&
                                provinceId != "tianjinshi" &&
                                provinceId != "chongqingshi")//如果是直辖市则不返回“直辖市”
                            {
                                result = item.Name;
                            }
                            break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(cityId))
            {
                if (AllCities != null && AllCities.Count > 0)
                {
                    foreach (var item in AllCities)
                    {
                        if (item.EntityType == 2 && item.Id == cityId)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result = item.Name;
                            }
                            else
                            {
                                result += " " + item.Name;
                            }
                            break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(areaId))
            {
                if (AllCities != null && AllCities.Count > 0)
                {
                    foreach (var item in AllCities)
                    {
                        if (item.EntityType == 3 && item.Id == areaId)
                        {
                            result += " " + item.Name;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        #region 三级联动
        /// <summary>
        /// 一级
        /// </summary>
        /// <returns></returns>
        public static List<City> GetProvice()
        {
            return AllCities.Where(v => v.IsActive && v.EntityType == 1).ToList();
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
                return AllCities.Where(v => v.IsActive && v.EntityType == 2 && v.ParentId != null && v.ParentId.Equals(provinceId)).ToList();
            }
        }

        /// <summary>
        /// 三级
        /// </summary>
        /// <returns></returns>
        public static List<City> GetArea(string cityId)
        {
            if (string.IsNullOrEmpty(cityId))
            {
                return new List<City>();
            }
            else
            {
                return AllCities.Where(v => v.IsActive && v.EntityType == 3 && v.ParentId != null && v.ParentId.Equals(cityId)).ToList();
            }
        }
        #endregion

        /// <summary>
        /// 过虑敏感词汇
        /// </summary>
        /// <param name="sensitivewords">过虑敏感</param>
        /// <returns>过虑后的文本</returns>
        public static string FilterSensitivewords(string sensitivewords)
        {
            if (!string.IsNullOrEmpty(sensitivewords))
            {
                SinglePageService singlePageService = new SinglePageService();
                SinglePage singlePage = singlePageService.GetSingPageById("51");
                if (singlePage != null && !string.IsNullOrEmpty(singlePage.ContentStyle))
                {
                    string[] sws = singlePage.ContentStyle.Split('+');
                    if (sws != null && sws.Count() > 0)
                    {
                        foreach (string sw in sws)
                        {
                            if (!string.IsNullOrEmpty(sw))
                            {
                                if (sensitivewords.Contains(sw))
                                {
                                    sensitivewords = sensitivewords.Replace(sw, "**");
                                }
                            }
                        }
                    }
                }
            }
            return sensitivewords;
        }
    }
}