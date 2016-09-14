using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Controllers;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.Account;
using Witbird.SHTS.Web.Models.User;
using ShortMessage = Witbird.SHTS.Model.ShortMessage;

namespace Witbird.SHTS.Web.Areas.M.Controllers
{
    public class AccountController : MobileBaseController
    {
        #region 字符串

        private const string REGMSGJS = "<script>alert('{0}');</script>";
        private static string VerifyMsg = ConfigurationManager.AppSettings["VerifyMsg"];

        #endregion

        #region 登录

        private UserService userService = null;

        public ActionResult Login()
        {
            LoginViewModel model=new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var errorMsg = string.Empty;
            try
            {
                User result = null;
                if (ModelState.IsValid)
                {
                    if (string.Equals(model.code,
                        Session["validataCode"].ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        userService = new UserService();
                        result = userService.Login(model.username, model.password);
                        if (result != null)
                        {
                            Session[USERINFO] = result;
                            return RedirectToAction("Index", "User", new { Area = "M" });
                        }
                        else
                        {
                            errorMsg = "用户名或密码错误！";
                        }
                    }
                    else
                    {
                        errorMsg = "验证码输入错误！";
                    }
                }
                else
                {
                    errorMsg = "信息填写有误！";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("注册用户", ex.ToString());
            }
            model.ErrorMsg = errorMsg;
            return View(model);
        }

        #endregion

        #region 注册

        public ActionResult Register()
        {
            var model = new UserRegisterViewModel { User = new User() };
            try
            {
                SinglePageService singlePageService = new SinglePageService();
                model.RegNotice = singlePageService.GetSingPageById("94");
            }
            catch (Exception)
            {
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(SHTS.Model.User user)
        {
            var userRegisterViewModel = new UserRegisterViewModel();
            try
            {
                userRegisterViewModel.User = user;
                var errorMsg = string.Empty;
                bool result = false;
                bool vcodeVail = false;
                //取消手机验证码逻辑
                //if (Session["vcode"] == null)
                //{
                //    errorMsg = "请先输入手机验证码！";
                //}
                //else if (!string.Equals(Session["vcode"].ToString(),
                //    Request.Form["code"], StringComparison.InvariantCultureIgnoreCase))
                //{
                //    errorMsg = "手机验证码输入错误！";
                //}
                //else
                {
                    vcodeVail = true;
                }
                if (vcodeVail)
                {
                    userService = new UserService();
                    user.LoginIdentiy = user.UserName;
                    user.Photo = ConfigurationManager.AppSettings["DefaultPhoto"];
                    //result = userService.UserRegister(user);
                    if (result)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        errorMsg = "注册失败";
                    }
                }
                if (!result)
                {
                    userRegisterViewModel.ErrorMsg = errorMsg;
                    SinglePageService singlePageService = new SinglePageService();
                    userRegisterViewModel.RegNotice = singlePageService.GetSingPageById("94");
                }
            }
            catch (Exception ex)
            {
                LogService.Log("注册用户", ex.ToString());
            }
            return View(userRegisterViewModel);
        }

        #endregion

        public RedirectResult LogOut()
        {
            try
            {
                var userAccount = new UserSummary
                {
                    UserName = UserInfo.UserName,
                    Password = null,
                    LastLoginTime = DateTime.Now
                };
                Cookie.Save("UserAccount", userAccount.ToJson());
                Clear();
            }
            catch (Exception ex)
            {
                LogService.Log("LogOut-失败", ex.ToString());
            }
            return Redirect("/m/account/login");
        }
    }
}
