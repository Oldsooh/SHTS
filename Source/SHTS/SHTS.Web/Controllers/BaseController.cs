﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected const string USERINFO = "userinfo";

        /// <summary>
        /// Gets the current logged on user information.
        /// </summary>
        public User UserInfo
        {
            get { return Session[USERINFO] as User; }
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

        public bool IsIdentified
        {
            get
            {
                bool isIdentified = false;
                if (UserInfo == null)
                {
                    isIdentified = false;
                }
                else
                {
                    try
                    {
                        UserService userService = new UserService();
                        User user = userService.GetUserById(UserInfo.UserId);
                        if (user != null)
                        {
                            isIdentified = (user.Vip.Value == (int)VipState.Identified || UserInfo.Vip.Value == (int)VipState.VIP);
                        }
                    }
                    catch
                    {

                    }
                }

                return isIdentified;
            }
        }

        public bool IsUserLogin
        {
            get { return UserInfo != null; }
        }

        public bool IsVip
        {
            get
            {
                bool isVip = false;

                if (UserInfo == null)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        UserService userService = new UserService();
                        User user = userService.GetUserById(UserInfo.UserId);
                        if (user != null)
                        {
                            isVip = (user.Vip.Value == (int)VipState.VIP);
                        }
                    }
                    catch
                    {

                    }
                }
                return isVip;
            }
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
    }
}