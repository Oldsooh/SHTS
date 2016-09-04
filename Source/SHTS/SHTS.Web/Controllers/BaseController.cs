using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Controllers
{
    public class BaseController : Controller
    {
        protected const string USERINFO = "userinfo";

        /// <summary>
        /// Gets the current logged on user information.
        /// </summary>
        public User UserInfo
        {
            get { return Session[USERINFO] as User; }
        }

        public virtual User CurrentUser
        {
            get
            {
                return UserInfo;
            }
            set
            {
                Session[USERINFO] = value;
            }
        }

        /// <summary>
        /// Clears the current logged on user information.
        /// </summary>
        public void RefreshUser()
        {
            var userService = new Witbird.SHTS.BLL.Services.UserService();
            Session[USERINFO] = userService.GetUserById(UserInfo.UserId);
        }

        /// <summary>
        /// Clears the current logged on user information.
        /// </summary>
        public void Clear()
        {
            Session[USERINFO] = null;
        }

        public virtual bool RequireLogin()
        {
            if (UserInfo == null)
            {
                Response.Redirect("/account/login");
                return true;
            }

            return false;
        }

        public virtual bool HasNormalPermission()
        {
            if (UserInfo == null)
            {
                return false;
            }
            return UserInfo.Vip>=0;
        }

        public virtual bool IsIdentified
        {
            get
            {
                if (IsUserLogin)
                {
                    return CurrentUser.IsIdentified;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual bool IsVip
        {
            get
            {
                if (IsUserLogin)
                {
                    return CurrentUser.IsVip;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual bool IsUserLogin
        {
            get { return CurrentUser != null; }
        }

        public string GetUrl(string subUrl)
        {
            string url = Request.Url.Authority;
            if (url.IndexOf("http://") < 0)
            {
                url = "http://" + url;
            }

            return url + subUrl;
        }

        /// <summary>
        /// 购买需求联系方式需要的价钱
        /// </summary>
        public decimal BuyDemandFee
        {
            get
            {
                decimal amount = 1m;//默认购买需要花费1元钱

                try
                {
                    if (!decimal.TryParse(ConfigurationManager.AppSettings["BuyDemandFee"], out amount))
                    {
                        amount = 1m;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogWexin("微信购买需求联系方式金额配置出错,请检查Web.config中<BuyDemandFee>配置", ex.ToString());
                    amount = 1m;
                }

                return amount;
            }
        }

        /// <summary>
        /// 更新默认城市地址
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public virtual bool UpdateDefaultCity(string cityId)
        {
            bool isSuccessful = false;

            if (CurrentUser.IsNotNull())
            {
                try
                {
                    var city = ConvertToCityObject(cityId);
                    if (city.IsNotNull())
                    {
                        UserService userService = new UserService();
                        CurrentUser.LocationId = city.ParentId + "," + city.Id + ",";

                        isSuccessful = userService.UserUpdate(CurrentUser);
                    }
                }
                catch(Exception ex)
                {
                    LogService.Log("UpdateDefaultCity", ex.ToString());
                }
            }

            return isSuccessful;
        }

        public virtual void SetDefaultCityToSession()
        {
            if (Session["CityId"] == null && CurrentUser.IsNotNull())
            {
                try
                {
                    var city = ConvertToCityObject(CurrentUser.City);
                    if (city.IsNotNull())
                    {
                        Session["CityId"] = city.Id;
                        Session["CityName"] = city.Name;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Log("UpdateDefaultCity", ex.ToString());
                }
            }
        }

        public City ConvertToCityObject(string cityId)
        {
            var allCities = Witbird.SHTS.Web.Public.StaticUtility.AllCities;
            if (allCities != null && allCities.Count > 0)
            {
                foreach (var item in allCities)
                {
                    if (item.EntityType == 2 && item.Id.Equals(cityId, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return item;
                    }
                }
            }

            return allCities.FirstOrDefault();
        }


        [HttpGet]
        public ActionResult VerifyCode()
        {
            //显示验证码
            string validataCode = null;
            ValidateCode_Style1 codeImg = new ValidateCode_Style1();
            byte[] bytes = codeImg.CreateImage(out validataCode);
            Session["validataCode"] = validataCode;
            return File(bytes, @"image/jpeg");
        }
    }
}
