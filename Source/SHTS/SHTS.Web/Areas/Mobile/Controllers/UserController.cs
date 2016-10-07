using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Controllers;
using Witbird.SHTS.Web.Models.User;
using Witbird.SHTS.Web.MvcExtension;

namespace Witbird.SHTS.Web.Areas.Mobile.Controllers
{
    public class UserController : MobileBaseController
    {
        //
        // GET: /M/User/
        OrderService orderService;
        DemandQuoteService quoteService;

        public UserController()
        {
            orderService = new OrderService();
            quoteService = new DemandQuoteService();
        }
        public ActionResult Index()
        {
            return View(CurrentUser);
        }

        public ActionResult ViewUser()
        {
            UserViewModel model = new UserViewModel { UserEntity = CurrentUser };
            try
            {
                if (!IsUserLogin)
                {
                    return Redirect("/mobile/account/login");
                }

                CityService cityService = new CityService();
                model.Provinces = cityService.GetProvinces(true);
                if (!string.IsNullOrEmpty(model.UserEntity.LocationId))
                {
                    model.Cities = cityService.GetCitiesByProvinceId(model.UserEntity.Province, true); //市
                    model.Areas = cityService.GetAreasByCityId(model.UserEntity.City, true); //区
                }
            }
            catch (Exception e)
            {
                LogService.Log("用户中心-viewuser", e.ToString());
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(User user)
        {
            try
            {
                UserService service = new UserService();
                var oldUser = service.GetUserById(UserInfo.UserId);
                if (oldUser != null)
                {
                    oldUser.Adress = user.Adress;
                    oldUser.QQ = user.QQ;
                    oldUser.LocationId = Request.Form["LocationId[]"];

                    if (!string.IsNullOrEmpty(Request.Form["Photo"]))
                    {
                        oldUser.Photo = Request.Form["Photo"];
                    }
                    if (service.UserUpdate(oldUser))
                    {
                        Session[USERINFO] = oldUser;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("会员列表", ex.ToString());
            }
            return RedirectToAction("ViewUser");
        }

        public ActionResult UpdatePassword()
        {
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdatePassword(string oldpassword, string newpassword)
        {
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            string message = null;
            try
            {
                if (string.Equals(oldpassword.ToMD5(),
                    model.UserEntity.EncryptedPassword,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    UserService service = new UserService();
                    model.UserEntity.EncryptedPassword = newpassword.ToMD5();
                    service.ResetUserPassword(model.UserEntity);
                }
                else
                {
                    message = "原密码输入错误！";
                }
            }
            catch (Exception e)
            {
                LogService.Log("UpdatePassword 出错了！", e.ToString());
                message = "密码修改失败！";
            }
            model.ErrorMsg = message;
            return View(model);
        }

        #region 会员认证

        /// <summary>
        /// 会员须知
        /// </summary>
        /// <returns></returns>
        public ActionResult ToVip()
        {
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            try
            {
                UserService service = new UserService();
                UserVip vipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);
                if (vipInfo != null && vipInfo.State.HasValue && vipInfo.State.Value == (int)VipState.VIP)
                {
                    return Redirect(GetUrl("/Mobile/user/VipInfo"));
                }

                SinglePageService singlePageService = new SinglePageService();
                model.ToVipNotice = singlePageService.GetSingPageById("95");
            }
            catch (Exception e)
            {
                LogService.Log("ToVip 出错了！", e.ToString());
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ToVip(FormCollection form)
        {
            try
            {
                UserService service = new UserService();
                //service.SetUserToVip(UserInfo);
            }
            catch (Exception e)
            {
                LogService.Log("ToVip 出错了！", e.ToString());
            }
            return View();
        }

        /// <summary>
        /// 上传证件
        /// </summary>
        /// <returns></returns>
        public ActionResult Identify(string returnUrl = null)
        {
            if (!IsUserLogin)
            {
                return Redirect("/Mobile/account/login");
            }

            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            try
            {
                UserService service = new UserService();
                model.VipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);
            }
            catch (Exception e)
            {
                LogService.Log("Identify 出错了！", e.ToString());
            }
            model.ErrorMsg = GetErrorMessage(model.VipInfo);

            ViewData["returnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult Identify(string identifyImgUrl, string returnUrl = null)
        {
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };

            if (!string.IsNullOrWhiteSpace(identifyImgUrl))
            {
                UserService service = new UserService();
                try
                {
                    UserVip vipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);

                    CurrentUser.IdentiyImg = identifyImgUrl;
                    CurrentUser.Vip = (int)VipState.Normal;
                    if (service.UserUpdate(CurrentUser) && service.UpdateUserVipInfo(vipInfo.Id, vipInfo.OrderId, identifyImgUrl,
                        DateTime.Now, DateTime.Now, vipInfo.Duration, vipInfo.Amount, VipState.Normal))
                    {
                        model.ErrorMsg = "照片上传成功，等待管理员审核";
                    }

                    // get it again
                    model.VipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);
                    if (string.IsNullOrEmpty(model.ErrorMsg))
                    {
                        model.ErrorMsg = GetErrorMessage(model.VipInfo);
                    }
                }
                catch (Exception e)
                {
                    LogService.Log("Identify 出错了！", e.ToString());
                    model.ErrorMsg = "认证照片上传失败！";
                }
            }
            else
            {
                model.ErrorMsg = "请选择认证照片！";
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return View(model);
        }

        /// <summary>
        /// vip会员费
        /// </summary>
        /// <returns></returns>
        //public ActionResult VipOrder()
        //{
        //    UserViewModel model = new UserViewModel { UserEntity = UserInfo ?? new User() };
        //    UserService service = new UserService();

        //    try
        //    {
        //        model.VipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);

        //        if (model.VipInfo != null)
        //        {
        //            if (!model.VipInfo.State.HasValue || model.VipInfo.State.Value == (int)VipState.Normal || model.VipInfo.State.Value == (int)VipState.Invalid)
        //            {
        //                return Redirect(GetUrl("/Mobile/user/Identify"));
        //            }
        //            else if (model.VipInfo.State.Value == (int)VipState.VIP)
        //            {
        //                return Redirect(GetUrl("/Mobile/user/VipInfo"));
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogService.Log("VipOrder 出错了！", e.ToString());
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult VipOrder(string vipDuration)
        //{
        //    string result = "生成VIP会员充值订单失败";

        //    if (UserInfo != null)
        //    {
        //        UserService userService = new UserService();

        //        try
        //        {
        //            if (UserInfo != null)
        //            {
        //                int duration = -1;
        //                decimal totalAmount = 0;

        //                int.TryParse(vipDuration, out duration);

        //                if (duration < 1 || duration > 5)
        //                {
        //                    throw new ArgumentException("充值VIP时间不正确，请重新选择");
        //                }

        //                totalAmount = 100 * duration;

        //                UserVip vipInfo = userService.GetUserVipInfoByUserId(UserInfo.UserId);

        //                if (vipInfo != null)
        //                {
        //                    if (vipInfo.State == (int)VipState.Normal || vipInfo.State == (int)VipState.Invalid)
        //                    {
        //                        throw new ArgumentException("认证资料还没有通过审核，请先上传认证资料");
        //                    }

        //                    TradeOrder order = null;
        //                    string url = GetUrl("/Mobile/user/VipInfo");

        //                    if (!string.IsNullOrEmpty(vipInfo.OrderId))
        //                    {
        //                        order = orderService.GetOrderByOrderId(vipInfo.OrderId);
        //                    }
        //                    if (order != null && order.UserName == UserInfo.UserName && order.Amount == totalAmount)
        //                    {
        //                        result = string.Format(Constant.PostPayInfoFormatForMobile, order.OrderId, url);
        //                    }
        //                    else
        //                    {
        //                        // 删掉原来的订单
        //                        if (order != null)
        //                        {
        //                            orderService.DeleteOrderById(order.OrderId);
        //                        }
        //                        string orderId = orderService.GenerateNewOrderNumber();
        //                        string subject = "活动在线网 | 用户VIP会员充值";
        //                        string body = "用户" + UserInfo.UserName + "充值VIP会员" + duration + "年";
        //                        int userId = UserInfo.UserId;
        //                        string username = UserInfo.UserName;
        //                        decimal amount = totalAmount;
        //                        int state = (int)OrderState.New;
        //                        string resourceUrl = url;

        //                        bool success = orderService.AddNewOrder(orderId, subject, body, amount, state, username, resourceUrl, (int)OrderType.ToVip, userId) &&
        //                            userService.UpdateUserVipInfo(vipInfo.Id, orderId, vipInfo.IdentifyImg, vipInfo.StartTime, vipInfo.EndTime, duration, totalAmount, VipState.Identified);

        //                        if (success)
        //                        {
        //                            result = string.Format(Constant.PostPayInfoFormatForMobile, orderId, url);
        //                        }
        //                        else
        //                        {
        //                            result = "生成VIP充值支付订单信息失败，请重新尝试";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (ArgumentException e)
        //        {
        //            result = e.Message;
        //        }
        //        catch (Exception e)
        //        {
        //            LogService.Log("生成VIP充值支付订单信息", e.ToString());
        //            result = "生成VIP充值支付订单信息，请重新尝试";
        //        }
        //    }
        //    else
        //    {
        //        result = "您还未登录或登录超时，请重新登录";
        //    }

        //    return Content(result);
        //}

        //public ActionResult VipInfo()
        //{
        //    UserViewModel model = new UserViewModel { UserEntity = UserInfo ?? new User() };
        //    UserService service = new UserService();

        //    try
        //    {
        //        model.VipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);

        //        if (model.VipInfo != null)
        //        {
        //            if (!model.VipInfo.State.HasValue || model.VipInfo.State.Value != (int)VipState.VIP)
        //            {
        //                return Redirect(GetUrl("/Mobile/user/ToVip"));
        //            }
        //            else
        //            {
        //                if (model.VipInfo.EndTime > DateTime.Now)
        //                {
        //                    model.ErrorMsg = "您的VIP已过期，请重新申请";
        //                    //service.SetUserToVip(
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogService.Log("VipInfo 出错了！", e.ToString());
        //    }

        //    return View(model);
        //}

        private string GetErrorMessage(UserVip vipInfo)
        {
            string errorMsg = string.Empty;

            if (vipInfo != null)
            {

                if (!string.IsNullOrEmpty(vipInfo.IdentifyImg))
                {
                    if (vipInfo.State.Value == (int)Witbird.SHTS.Model.VipState.Normal)
                    {
                        errorMsg = "认证资料已上传，管理员正在审核中";
                    }
                    else if (vipInfo.State.Value == (int)Witbird.SHTS.Model.VipState.Invalid)
                    {
                        errorMsg = "认证资料审核未通过，请重新上传";
                    }
                    else if (vipInfo.State.Value == (int)Witbird.SHTS.Model.VipState.Identified)
                    {
                        errorMsg = "认证资料已通过审核，您现在是认证会员。还可访问电脑版(www.huodongzaixian.com)升级为VIP会员，享受更多会员特权！";
                    }
                    else if (vipInfo.State.Value == (int)Witbird.SHTS.Model.VipState.VIP)
                    {
                        errorMsg = "认证资料已通过审核，您是VIP会员";
                    }
                    else
                    {
                        errorMsg = "数据错误，请联系网站管理员";
                    }
                }
            }

            return errorMsg;
        }

        #endregion
    }
}
