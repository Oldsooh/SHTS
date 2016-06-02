using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Controllers
{
    public class CityController : Controller
    {
        CityService cityService = new CityService();

        public ActionResult Index()
        {
            CityModel model = new CityModel();
            model.Provinces = cityService.GetProvinces(true);
            return View(model);
        }

        public ActionResult Current(string id)
        {
            if (!string.IsNullOrEmpty(id) && id.Equals("china"))
            {
                Session["CityId"] = null;
                Session["CityName"] = "全国";
            }
            if (Witbird.SHTS.Web.Public.StaticUtility.AllCities != null && Witbird.SHTS.Web.Public.StaticUtility.AllCities.Count > 0)
            {
                foreach (var item in Witbird.SHTS.Web.Public.StaticUtility.AllCities)
                {
                    if (item != null && item.Id == id)
                    {
                        Session["CityId"] = item.Id;
                        Session["CityName"] = item.Name;
                    }
                }
            }
            if (Request.UrlReferrer.AbsolutePath == "/city/")
            {
                return Redirect("/");
            }
            else
            {
                return Redirect(Request.UrlReferrer.LocalPath);
            }
        }

        [HttpPost]
        public ActionResult Cities(string provinceId)
        {
            List<City> cities = cityService.GetCitiesByProvinceId(provinceId, true);
            if (cities == null)
            {
                cities = new List<City>();
            }
            var data = new
            {
                total = cities.Count,
                rows = from s in cities
                       select new
                       {
                           s.Id,
                           s.Name
                       }
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Areas(string cityId)
        {
            List<City> areas = cityService.GetAreasByCityId(cityId, true);
            if (areas == null)
            {
                areas = new List<City>();
            }
            var data = new
            {
                total = areas.Count,
                rows = from s in areas
                       select new
                       {
                           s.Id,
                           s.Name
                       }
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //根据IP地址获取城市名，再根据城市名称设置当前城市
        [HttpGet]
        public ActionResult SetLocalCity(string cityName)
        {
            string result = "no";

            if (!string.IsNullOrEmpty(cityName) && Session["CityId"] == null)
            {
                if (Witbird.SHTS.Web.Public.StaticUtility.AllCities != null && Witbird.SHTS.Web.Public.StaticUtility.AllCities.Count > 0)
                {
                    var allCities = Witbird.SHTS.Web.Public.StaticUtility.AllCities;
                    if (allCities != null && allCities.Count > 0)
                    {
                        foreach (var item in allCities)
                        {
                            if (item.EntityType == 2 && item.Name == cityName)
                            {
                                Session["CityId"] = item.Id;
                                Session["CityName"] = item.Name;
                                result = item.Name;
                                break;
                            }
                        }

                        if (result == "no")
                        {
                            var firstCity = allCities.FirstOrDefault();
                            Session["CityId"] = firstCity.Id;
                            Session["CityName"] = firstCity.Name;
                            result = firstCity.Name;
                        }
                    }
                }
            }

            return Content(result);
        }
    }
}
