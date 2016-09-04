using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.Account;
using Witbird.SHTS.Web.Models.User;
using Witbird.SHTS.Web.Public;

namespace Witbird.SHTS.Web.Controllers
{
    public class AccountController : PCBaseController
    {
        #region 字符串

        private const string REGMSGJS = "<script>alert('{0}');</script>";
        private static string VerifyMsg = ConfigurationManager.AppSettings["VerifyMsg"];

        #endregion

        private UserService userService = null;

        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

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
        public ActionResult Login(LoginViewModel model)
        {
            var errorMsg = string.Empty;
            try
            {
                User result = null;
                userService = new UserService();

                if (string.Equals(model.IsAutoLogin, "0"))
                {
                    #region 正常登陆

                    if (string.Equals(model.code,
                        Session["validataCode"].ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = userService.Login(model.username, model.password);
                        if (result != null)
                        {
                            Session[USERINFO] = result;
                            SaveUserAccountToCookie(model);
                            return RedirectToAction("index", "Home");
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
                    #endregion
                }
                else
                {
                    #region 自动登陆

                    model.password = Encrypt.Decode(model.password);
                    result = userService.Login(model.username, model.password);
                    if (result != null)
                    {
                        Session[USERINFO] = result;
                        SaveUserAccountToCookie(model);
                        return RedirectToAction("index", "Home");
                    }
                    else
                    {
                        errorMsg = "自动登陆失败！";
                    }

                    #endregion
                }

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    Cookie.Remove("UserAccount");
                }
            }
            catch (Exception ex)
            {
                LogService.Log("用户登陆", ex.ToString());
            }
            model.ErrorMsg = errorMsg;
            return View(model);
        }

        private void SaveUserAccountToCookie(LoginViewModel model)
        {
            if (string.Equals(model.RemberAccount, "on",
                StringComparison.InvariantCultureIgnoreCase))
            {
                var userAccount = new UserSummary
                {
                    UserName = HttpUtility.UrlEncode(model.username),
                    Password = Encrypt.Encode(model.password),
                    LastLoginTime = DateTime.Now
                };
                Cookie.Save("UserAccount", userAccount.ToJson());
            }
            else
            {
                Cookie.Remove("UserAccount");
            }
        }

        public ActionResult Register()
        {
            var model = new UserRegisterViewModel {User = new User()};
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
                //else if(!string.Equals(Session["vcode"].ToString(),
                //    Request.Form["code"],StringComparison.InvariantCultureIgnoreCase))
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
                    //string imgUrl = null;
                    //errorMsg = FileUploadHelper.SaveFile(this.HttpContext, "IdentiyImg", out imgUrl);
                    user.LoginIdentiy = user.UserName;
                    //user.IdentiyImg = imgUrl;
                    user.Photo = ConfigurationManager.AppSettings["DefaultPhoto"];
                    result = userService.UserRegister(user);
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
            catch(Exception ex)
            {
                LogService.Log("注册用户",ex.ToString());
            }
            return View(userRegisterViewModel);
        }

        public RedirectResult LogOut()
        {
            try
            {
                var userInfoInCookie = Cookie.GetValue("UserAccount");
                var userAccount = 
                    JsonConvert.DeserializeObject<UserSummary>(userInfoInCookie);
                if (userAccount == null)
                {
                    userAccount = new UserSummary
                    {
                        UserName = HttpUtility.HtmlEncode(UserInfo.UserName),
                        Password = null,
                        LastLoginTime = DateTime.Now
                    };
                }
                else
                {
                    userAccount.Password = null;
                    userAccount.LastLoginTime = DateTime.Now;
                }
                Cookie.Save("UserAccount", userAccount.ToJson());
                Clear();
            }
            catch (Exception ex)
            {
                LogService.Log("LogOut-失败", ex.ToString());
            }
            return Redirect("~/account/login");
        }

        #region Not use now Reset password via phone number verification.

        public ActionResult ForGetPassowrd()
        {
            return View(new GetBackPasswordViewModel());
        }

        [HttpPost]
        public ActionResult ForGetPassowrd(GetBackPasswordViewModel getBackPasswordViewModel)
        {
            try
            {
                var errorMsg = string.Empty;
                bool result = false;
                bool vcodeVail = true;
                if (Session["vcode"] == null)
                {
                    errorMsg = "请先输入手机验证码！";
                    vcodeVail = false;
                }
                else if (!string.Equals(Session["vcode"].ToString(),
                    getBackPasswordViewModel.CellPhoneVCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    errorMsg = "手机验证码输入错误！";
                    vcodeVail = false;
                }
                if (vcodeVail)
                {
                    if (!string.Equals(Session["validataCode"].ToString(),
                    getBackPasswordViewModel.VCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        errorMsg = "验证码输入错误！";
                        vcodeVail = false;
                    }
                }
                if (vcodeVail)
                {
                    userService = new UserService();
                    User user=new User
                    {
                        Cellphone = getBackPasswordViewModel.CellPhone,
                        EncryptedPassword = getBackPasswordViewModel.EncryptedPassword.ToMD5()
                    };
                    result = userService.ResetUserPassword(user);
                    if (result)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        errorMsg = "该号码未注册！";
                    }
                }
                if (!result)
                {
                    getBackPasswordViewModel.Message = errorMsg;
                }
            }
            catch (Exception ex)
            {
                LogService.Log("修改密码", ex.ToString());
                getBackPasswordViewModel.Message = "修改密码失败";
            }
            return View(getBackPasswordViewModel);
        }

        #endregion

        #region Reset password via mail verification.

        public ActionResult ResetAccountPassword()
        {
            ViewData["Email"] = string.Empty;
            ViewData["ErrorMessage"] = string.Empty;
            return View();
        }

        [HttpPost]
        public ActionResult ResetAccountPassword(string email, string vCode)
        {
            string errorMessage = string.Empty;

            try
            {
                userService = new UserService();

                if (!string.Equals(Session["validataCode"].ToString(),
                        vCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    errorMessage = "验证码输入错误！";
                }
                else if (userService.VerifyUserInfo("Email", email))
                {
                    errorMessage = "该邮箱尚未注册";
                }
                else
                {
                    User user = userService.GetUserByUserName(email);

                    if (user != null)
                    {
                        var newPassword = GetRandom();
                        user.EncryptedPassword = newPassword.ToMD5();

                        if (userService.ResetUserPassword(user))
                        {
                            SinglePageService singlePageService = new SinglePageService();
                            var mailContentFormat = singlePageService.GetSingPageById("10").ContentStyle;
                            var mailContent = string.Format(mailContentFormat, newPassword);
                            var resetPasswordMail = StaticUtility.EmailManager.CreateMailMessage(email, "活动在线网", "密码重置-活动在线网", mailContent);
                            var response = StaticUtility.EmailManager.Send(resetPasswordMail);
                            if (response.IsSuccess)
                            {
                                //密码重置邮件已发送至您的邮箱，请注意查收
                                errorMessage = "success";
                            }
                            else
                            {
                                LogService.Log("密码重置邮件发送失败", response.InnerException.ToString());
                                errorMessage = "密码重置邮件发送失败，请重新尝试";
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "用户信息不存在，请确认后重试";
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.Log("密码重置发生异常", ex.ToString());
            }

            ViewData["Email"] = email;
            ViewData["ErrorMessage"] = errorMessage;

            return View();
        }

        #endregion

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

        [HttpPost]
        public JsonResult VerifyBeforeVcode(string vcode)
        {
            AjaxResult result=new AjaxResult();
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
