using System;
using System.Configuration;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.Account;
using Witbird.SHTS.Web.Models.User;

namespace Witbird.SHTS.Web.Areas.Mobile.Controllers
{
    public class AccountController : MobileBaseController
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
            LoginViewModel model = new LoginViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
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
                            CurrentUser = user;
                            return RedirectToAction("Index", "User", new { Area = "Mobile" });
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
                    errorMsg = "用户名或密码错误！";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("用户登录", ex.ToString());
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
            catch (Exception ex)
            {
                LogService.LogWexin("会员注册", ex.ToString());
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
                    user.Photo = ConfigurationManager.AppSettings["DefaultPhoto"];
                    result = userService.UserRegister(user, out errorMsg);

                    if (result)
                    {
                        //return Redirect("/mobile/account/login");
                        errorMsg = "SUCCESS";
                        CurrentUser = user;
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

        #region 退出登录

        public ActionResult LogOut()
        {
            try
            {
                var userAccount = new UserSummary
                {
                    UserName = UserInfo.UserName,
                    Password = null,
                    LastLoginTime = DateTime.Now
                };
                Clear();
            }
            catch (Exception ex)
            {
                LogService.Log("LogOut-失败", ex.ToString());
            }
            return Redirect("/mobile/user/index");
        }
        #endregion 退出登录

        /// <summary>
        /// 检测用户名是否已被注册。
        /// </summary>
        /// <param name="username"></param>
        /// <param name="cellphone"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult VerifyUserName(string field, string value)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                userService = new UserService();
                if (!userService.VerifyUserInfo(field, value))
                {
                    result.ExceptionInfo = "该用户名已经被注册！";
                    result.ErrorCode = 103;
                }
            }
            catch (Exception ex)
            {
                LogService.Log("VerifyUserName", ex.ToString());
                result.ExceptionInfo = "出错了！";
                result.ErrorCode = 102;
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult VerifyCellphone(string cellphone)
        {
            AjaxResult result = new AjaxResult();
            //try
            //{
            //    var vcode = GetRandom();
            //    ISendShortMessageService smsSerive = SMSServiceFactory.Create();
            //    var message=new ShortMessage { 
            //        ToPhoneNumber=cellphone,
            //        Content = vcode,
            //        Parameters = new []{vcode,"300"}
            //    };
            //    var response = smsSerive.SendShortMessage(message);
            //    if (response.IsSuccess)
            //    {
            //        Session["vcode"] = vcode;
            //    }
            //    else
            //    {
            //        result.ExceptionInfo = "发送验证码失败";
            //        result.ErrorCode = 403; 
            //    }
            //    InnerSMSService smService = new InnerSMSService();
            //    smService.AddSMSRecordAsync(new Witbird.SHTS.Model.ShortMessage
            //    {
            //        Cellphone = message.ToPhoneNumber,
            //        State = response.IsSuccess? 0:1
            //    });
            //}
            //catch (Exception ex)
            //{
            //    LogService.Log("发送验证码", ex.ToString());
            //    result.ExceptionInfo = "发送验证码失败";
            //    result.ErrorCode = 102;
            //}
            return Json(result);
        }

        [HttpPost]
        public JsonResult VerifyBeforeVcode(string vcode)
        {
            AjaxResult result = new AjaxResult();
            if (!string.Equals(vcode,
                Session["validataCode"].ToString(),
                StringComparison.InvariantCultureIgnoreCase))
            {
                result.ExceptionInfo = "error";
            }
            return Json(result);
        }

        private string GetRandom()
        {
            const string formatString = "1,2,3,4,5,6,7,8,9,0";
            string codeString = string.Empty;
            string[] strArray = formatString.Split(new char[] { ',' });
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                int index = random.Next(0x186a0) % strArray.Length;
                codeString = codeString + strArray[index].ToString();
            }
            return codeString;
        }
    }
}
