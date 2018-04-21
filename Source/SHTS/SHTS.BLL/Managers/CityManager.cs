using System;
using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Managers
{
    public class CityManager
    {
        private CityRepository cityRepository;

        public CityManager()
        {
            cityRepository = new CityRepository();
        }

        public City GetCityById(string id)
        {
            City city = null;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    city = cityRepository.FindOne(c => c.Id.Equals(id));
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询单个城市失败", e.ToString());
            }
            return city;
        }

        public List<City> GetCities(bool isALL)
        {
            List<City> result = null;
            try
            {
                List<City> cities = cityRepository.FindAll().OrderBy(c => c.Sort).ToList();
                if (cities != null && cities.Count > 0)
                {
                    result = new List<City>();
                    foreach (City parent in cities)
                    {
                        if (string.IsNullOrEmpty(parent.ParentId))
                        {
                            if (isALL)
                            {
                                result.Add(parent);
                                foreach (City child in cities)
                                {
                                    if (child.ParentId == parent.Id)
                                    {
                                        result.Add(child);
                                    }
                                }
                            }
                            else
                            {
                                if (parent.IsActive)
                                {
                                    result.Add(parent);
                                    foreach (City child in cities)
                                    {
                                        if (child.IsActive && child.ParentId == parent.Id)
                                        {
                                            result.Add(child);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询城市列表失败", e.ToString());
            }
            return result;
        }

        public string AddCity(City city)
        {
            string result = "添加失败";
            try
            {
                if (city != null)
                {

                    if (!string.IsNullOrEmpty(city.Id) && !string.IsNullOrEmpty(city.Name))
                    {
                        City oldCity = GetCityById(city.Id);
                        if (oldCity == null)
                        {
                            if (cityRepository.AddEntitySave(city))
                            {
                                result = "success";
                            }
                        }
                        else
                        {
                            result = "该城市已存在"; 
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                LogService.Log("添加城市失败", e.ToString());
            }
            return result;
        }

        public bool EditCity(City city)
        {
            bool result = false;
            try
            {
                if (city != null)
                {
                    if (!string.IsNullOrEmpty(city.Id) && !string.IsNullOrEmpty(city.Name))
                    {
                        result = cityRepository.UpdateEntitySave(city);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("编辑城市失败", e.ToString());
            }
            return result;
        }

        public bool DeleteCity(City city)
        {
            bool result = false;
            try
            {
                if (city != null)
                {
                    if (!string.IsNullOrEmpty(city.Id) && !string.IsNullOrEmpty(city.Name))
                    {
                        result = cityRepository.DeleteEntitySave(city);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("删除城市失败", e.ToString());
            }
            return result;
        }

        public bool HasChild(string id)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    result = cityRepository.FindAll().Where(c => c.ParentId == id).ToList().Count > 0;
                }
            }
            catch (Exception e)
            {
                LogService.Log("删除城市失败", e.ToString());
            }
            return result;
        }
    }
}
