using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class CityController : AdminBaseController
    {
        CityManager cityManager = new CityManager();
        CityService cityService = new CityService();

        public ActionResult Index(string provinceId)
        {
            CityModel model = new CityModel();
            model.Provices = cityService.GetProvinces(true); //用于添加页下拉选择
            model.Province = string.Empty;//当前选择省

            if (!string.IsNullOrEmpty(provinceId))
            {
                //获取省下面所有一级城市以及区县
                model.Cities = cityService.GetAllCitiesOfProvince(provinceId);
                model.Province = provinceId;
                model.SelectCities = cityService.GetCitiesByProvinceId(provinceId, true); //用于添加页下拉选择
            }
            else
            {
                if (Witbird.SHTS.Web.Public.StaticUtility.AllCities != null && Witbird.SHTS.Web.Public.StaticUtility.AllCities.Count > 0)
                {
                    model.Cities = new List<City>();
                    foreach (var item in Witbird.SHTS.Web.Public.StaticUtility.AllCities)
                    {
                        if (item.EntityType == 2 && item.Sort < 100)
                        {
                            model.Cities.Add(item);
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [Permission(EnumRole.Editer)]
        public ActionResult Add(string id, string name, string provinceId, string parentId, string sort, string isActive)
        {
            string result = "添加失败";

            if (!string.IsNullOrEmpty(id) &&
                !string.IsNullOrEmpty(name) &&
                !string.IsNullOrEmpty(sort) &&
                !string.IsNullOrEmpty(isActive))
            {
                City city = new City();
                city.Id = id;
                city.Name = name;
                city.EntityType = string.IsNullOrEmpty(parentId) ? 2 : 3;
                city.ParentId = string.IsNullOrEmpty(parentId) ? provinceId : parentId;
                int tempSort = 101;
                Int32.TryParse(sort, out tempSort);
                city.Sort = tempSort;
                city.IsActive = isActive == "true";
                result = cityManager.AddCity(city);
                Witbird.SHTS.Web.Public.StaticUtility.UpdateCities();
            }
            else
            {
                result = "必填项不能为空";
            }

            if (result == "success")
            {
                result = Request.UrlReferrer.AbsoluteUri;
            }

            return Content(result);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult Active(string id, string isActive)
        {
            if (!string.IsNullOrEmpty(isActive))
            {
                City city = cityManager.GetCityById(id);
                city.IsActive = isActive.Equals("true");
                if (cityManager.EditCity(city))
                {
                    Witbird.SHTS.Web.Public.StaticUtility.UpdateCities();
                }
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult Delete(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                City city = cityManager.GetCityById(id);
                if (string.IsNullOrEmpty(city.ParentId))
                {
                    if (cityManager.HasChild(id))
                    {
                        return Redirect(Request.UrlReferrer.AbsoluteUri);
                    }
                }
                if (cityManager.DeleteCity(city))
                {
                    Witbird.SHTS.Web.Public.StaticUtility.UpdateCities();
                }
            }

            //return Content(result);
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}
