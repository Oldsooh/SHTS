using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Controllers;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.Account;
using Witbird.SHTS.Web.Models.User;
using ShortMessage = Witbird.SHTS.Model.ShortMessage;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class AccountController : WechatBaseController
    {
        #region 字符串

        private const string REGMSGJS = "<script>alert('{0}');</script>";
        private static string VerifyMsg = ConfigurationManager.AppSettings["VerifyMsg"];

        #endregion

        #region 登录

        private UserService userService = null;

        public AccountController()
        {
            userService = new UserService();
        }

        public ActionResult Login()
        {
            WeChatLoginViewModel model = new WeChatLoginViewModel();

            try
            {
                if (CurrentWeChatUser.UserId.HasValue)
                {
                    var user = userService.GetUserById(CurrentWeChatUser.UserId.Value);
                    model.User = user;
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("账号绑定GET", ex.ToString());
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Login(WeChatLoginViewModel model)
        {
            var errorMsg = string.Empty;
            try
            {
                User user = null;
                if (ModelState.IsValid)
                {
                    if (string.Equals(model.code,
                        Session["validataCode"].ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (CurrentWeChatUser.UserId.HasValue && CurrentUser != null)
                        {
                            errorMsg = "当前已绑定会员账号（" + CurrentUser.UserName + ")，请先解除绑定后重试。";
                        }
                        else
                        {
                            user = userService.Login(model.username, model.password);
                            if (user != null)
                            {
                                WeChatUser loggedInUser = userService.GetWeChatUser(user.UserId);

                                if (loggedInUser == null)
                                {
                                    CurrentWeChatUser.UserId = user.UserId;

                                    if (userService.UpdateWeChatUser(CurrentWeChatUser))
                                    {
                                        CurrentUser = user;
                                        return RedirectToAction("Index", "User", new { Area = "Wechat" });
                                    }
                                    else
                                    {
                                        errorMsg = "绑定用户失败！";
                                    }
                                }
                                else
                                {
                                    errorMsg = "该账号已绑定微信号(" + loggedInUser.NickName + ")， 请先解绑该微信号后重试。";
                                }
                            }
                            else
                            {
                                errorMsg = "用户名或密码错误！";
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "验证码输入错误！";
                    }

                    if (CurrentWeChatUser.UserId.HasValue)
                    {
                        user = userService.GetUserById(CurrentWeChatUser.UserId.Value);
                        model.User = user;
                    }
                }
                else
                {
                    errorMsg = "信息填写有误！";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("账号绑定POST", ex.ToString());
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
                if (CurrentWeChatUser.UserId.HasValue && CurrentUser != null)
                {
                    model.ErrorMsg = "UserLoggedIn";
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("微信会员注册页面访问异常", ex.ToString());
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
                //取消手机验证逻辑
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
                    user.LoginIdentiy = user.UserName;
                    user.Photo = !string.IsNullOrEmpty(CurrentWeChatUser.Photo) ? CurrentWeChatUser.Photo : ConfigurationManager.AppSettings["DefaultPhoto"];
                    result = userService.WeChatUserRegister(user, CurrentWeChatUser.OpenId);

                    if (result)
                    {
                        //return Redirect("/wechat/account/login");
                        errorMsg = "SUCCESS";
                    }
                    else
                    {
                        errorMsg = "注册会员账号失败";
                    }
                }

                userRegisterViewModel.ErrorMsg = errorMsg;

                if (!result)
                {
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

        #region 解除账号绑定

        public ActionResult LogOut()
        {
            WeChatLoginViewModel model = new WeChatLoginViewModel();

            if (CurrentUser != null)
            {
                model.username = CurrentUser.UserName;
            }

            try
            {
                if (!CurrentWeChatUser.UserId.HasValue)
                {
                    model.ErrorMsg = "logoutinvalid";
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("解除账号绑定GET", ex.ToString());
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult LogOut(WeChatLoginViewModel model)
        {
            var errorMsg = string.Empty;
            try
            {
                User user = null;
                if (ModelState.IsValid)
                {
                    if (string.Equals(model.code,
                        Session["validataCode"].ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        user = userService.Login(model.username, model.password);
                        if (user != null)
                        {
                            if (user.UserId != CurrentWeChatUser.UserId.Value)
                            {
                                errorMsg = "请使用当前微信绑定的会员账户登录。";
                            }
                            else
                            {
                                CurrentWeChatUser.UserId = null;

                                if (userService.UpdateWeChatUser(CurrentWeChatUser))
                                {
                                    CurrentUser = null;
                                    errorMsg = "logoutsuccess";
                                }
                                else
                                {
                                    errorMsg = "解除绑定失败，请重试。";
                                }
                            }
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

                    if (CurrentWeChatUser.UserId.HasValue)
                    {
                        user = userService.GetUserById(CurrentWeChatUser.UserId.Value);
                        model.User = user;
                    }
                }
                else
                {
                    errorMsg = "信息填写有误！";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("解除账号绑定POST", ex.ToString());
            }

            model.ErrorMsg = errorMsg;
            return View(model);
        }

        #endregion 解除账号绑定
    }
}
