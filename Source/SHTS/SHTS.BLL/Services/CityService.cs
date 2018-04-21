using System;
using System.Collections.Generic;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    public class CityService
    {
        private CityDao cityDao;

        public CityService()
        {
            cityDao = new CityDao();
        }

        /// <summary>
        /// 查询所有城市，包括省、市、区
        /// </summary>
        /// <param name="isNeedActive">是否只需要启用城市</param>
        public List<City> GetCities(bool isNeedActive)
        {
            List<City> cities = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                List<City> tempCities = null;
                tempCities = cityDao.SelectCites(conn);
                if (tempCities != null && tempCities.Count > 0)
                {
                    cities = new List<City>();
                    foreach (var item in tempCities)
                    {
                        if (isNeedActive)
                        {
                            if (item.IsActive)
                            {
                                cities.Add(item);
                            }
                        }
                        else
                        {
                            cities.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询城市列表", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return cities;
        }

        /// <summary>
        /// 查询所有省
        /// </summary>
        /// <param name="isNeedActive">是否只需要启用省份</param>
        public List<City> GetProvinces(bool isNeedActive)
        {
            List<City> cities = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                List<City> tempCities = null;
                tempCities = cityDao.SelectProvinces(conn);
                if (tempCities != null && tempCities.Count > 0)
                {
                    cities = new List<City>();
                    foreach (var item in tempCities)
                    {
                        if (isNeedActive)
                        {
                            if (item.IsActive)
                            {
                                cities.Add(item);
                            }
                        }
                        else
                        {
                            cities.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询所有省", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return cities;
        }

        /// <summary>
        /// 获取份省下面的所有一级城市
        /// </summary>
        /// <param name="provinceId">省份Id</param>
        /// <param name="isNeedActive">是否只需要启用城市</param>
        /// <returns></returns>
        public List<City> GetCitiesByProvinceId(string provinceId, bool isNeedActive)
        {
            List<City> cities = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                List<City> tempCities = null;
                tempCities = cityDao.SelectCitesByProvinceId(provinceId, conn);
                if (tempCities != null && tempCities.Count > 0)
                {
                    cities = new List<City>();
                    foreach (var item in tempCities)
                    {
                        if (isNeedActive)
                        {
                            if (item.IsActive)
                            {
                                cities.Add(item);
                            }
                        }
                        else
                        {
                            cities.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("获取份省下面的所有一级城市", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return cities;
        }

        /// <summary>
        /// 获取该省下面所有城市、区县
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public List<City> GetAllCitiesOfProvince(string provinceId)
        {
            List<City> cities = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();

                List<City> tempCities = null;//省下面的一级城市
                tempCities = cityDao.SelectCitesByProvinceId(provinceId, conn);

                if (tempCities != null && tempCities.Count > 0)
                {
                    cities = new List<City>();
                    foreach (var city in tempCities)
                    {
                        cities.Add(city);
                        List<City> areas = null;//一级城市下面的区、县
                        areas = cityDao.SelectCitesByCityId(city.Id, conn);
                        if (areas != null && areas.Count > 0)
                        {
                            foreach (var area in areas)
                            {
                                cities.Add(area);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("获取该省下面所有城市、区县", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return cities;
        }

        /// <summary>
        /// 根据一级城市Id查询所有该市下的二级城市、区、商圈
        /// </summary>
        /// <param name="cityId">一级城市Id</param>
        /// <param name="isNeedActive">是否只需要启用城市</param>
        /// <returns></returns>
        public List<City> GetAreasByCityId(string cityId, bool isNeedActive)
        {
            List<City> cities = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                List<City> tempCities = null;
                tempCities = cityDao.SelectCitesByCityId(cityId, conn);
                if (tempCities != null && tempCities.Count > 0)
                {
                    cities = new List<City>();
                    foreach (var item in tempCities)
                    {
                        if (isNeedActive)
                        {
                            if (item.IsActive)
                            {
                                cities.Add(item);
                            }
                        }
                        else
                        {
                            cities.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询二级城市、区、商圈", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return cities;
        }
    }
}
