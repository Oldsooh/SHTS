using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Html;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.ActivityModel;
using Witbird.SHTS.Web.Models.User;
using Witbird.SHTS.Web.MvcExtension;

namespace Witbird.SHTS.Web.Controllers
{
    public class UserController : BaseController
    {
        OrderService orderService;
        public UserController()
        {
            orderService = new OrderService();
        }

        //
        // GET: /User/

        /// <summary>
        /// 用户中心
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (UserInfo == null)
            {
                return RedirectToAction("login", "account");
            }
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            try
            {
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
                LogService.Log("用户中心-index", e.ToString());
            }
            return View(model);
        }

        #region 用户信息

        [HttpPost]
        public ActionResult UpdateBasic(Model.User user)
        {
            try
            {
                UserService service = new UserService();
                var oldUser = service.GetUserById(UserInfo.UserId);
                if (oldUser != null)
                {
                    oldUser.Adress = user.Adress;
                    oldUser.QQ = user.QQ;
                    oldUser.SiteUrl = user.SiteUrl;
                    oldUser.LocationId = Request.Form["LocationId[]"];

                    if (Request.Files["Photo"] != null &&
                        !string.IsNullOrEmpty(Request.Form["IdentiyImgFile"]))
                    {
                        string imgUrl = null;
                        FileUploadHelper.SaveFile(this.HttpContext, "Photo", out imgUrl);
                        oldUser.Photo = imgUrl;
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
            return RedirectToAction("Index");
        }

        public ActionResult Profile()
        {
            RequireLogin();
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            try
            {
                UserService service = new UserService();
                var userProfiles = service.GetUserProfiles(model.UserEntity.UserId);
                if (userProfiles != null && userProfiles.Count > 0)
                {
                    model.UserProfiles = userProfiles.ToDictionary(profile => (profile.ProfileType));
                }
                else
                {
                    model.UserProfiles = new Dictionary<string, UserProfile>();
                }
            }
            catch (Exception ex)
            {
                LogService.Log("会员列表", ex.ToString());
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Profile(string description)
        {
            UserViewModel model = new UserViewModel();
            try
            {
                UserService service = new UserService();
                model.UserEntity = UserInfo;
                model.description = description;

                var userId = model.UserEntity.UserId.ToString();
                var profileList = new List<UserProfile>
                {
                    new UserProfile
                    {
                        UserId = userId,
                        ProfileType = "description",
                        Value = model.description
                    }
                };
                service.UpdateUserProfile(profileList);
                RefreshUser();
            }
            catch (Exception ex)
            {
                LogService.Log("会员列表", ex.ToString());
            }
            return View(model);
        }


        #region 会员认证

        /// <summary>
        /// 会员须知
        /// </summary>
        /// <returns></returns>
        public ActionResult ToVip()
        {
            RequireLogin();
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            try
            {
                UserService service = new UserService();
                UserVip vipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);
                if (vipInfo != null && vipInfo.State.HasValue && vipInfo.State.Value == (int)VipState.VIP)
                {
                    return Redirect(GetUrl("/user/VipInfo"));
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
        public ActionResult Identify()
        {
            RequireLogin();
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
            return View(model);
        }

        [HttpPost]
        public ActionResult Identify(FormCollection form)
        {
            RequireLogin();
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            UserService service = new UserService();
            try
            {
                UserVip vipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);

                string errorMsg = string.Empty;
                string imgUrl = string.Empty;
                try
                {
                    HttpPostedFileBase postFile = this.HttpContext.Request.Files["IdentiyImg"];
                    using (System.Drawing.Image img = System.Drawing.Image.FromStream(postFile.InputStream))
                    {
                    }

                }
                catch
                {
                    errorMsg = "图片格式错误，请重新选择";
                }

                if (string.IsNullOrEmpty(errorMsg))
                {
                    errorMsg = FileUploadHelper.SaveFile(this.HttpContext, "IdentiyImg", out imgUrl);
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        UserInfo.IdentiyImg = imgUrl;
                        UserInfo.Vip = (int)VipState.Normal;
                        if (service.UserUpdate(UserInfo) && service.UpdateUserVipInfo(vipInfo.Id, vipInfo.OrderId, imgUrl,
                            DateTime.Now, DateTime.Now, vipInfo.Duration, vipInfo.Amount, VipState.Normal))
                        {
                            errorMsg = "图片上传成功，等待管理员审核";
                        }
                    }
                }

                // get it again
                model.VipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);
                if (string.IsNullOrEmpty(errorMsg))
                {
                    errorMsg = GetErrorMessage(model.VipInfo);
                }
                model.ErrorMsg = errorMsg;
            }
            catch (Exception e)
            {
                LogService.Log("Identify 出错了！", e.ToString());
            }

            model.VipInfo = model.VipInfo ?? service.GetUserVipInfoByUserId(UserInfo.UserId);
            return View(model);
        }

        /// <summary>
        /// vip会员费
        /// </summary>
        /// <returns></returns>
        public ActionResult VipOrder()
        {
            UserViewModel model = new UserViewModel { UserEntity = UserInfo ?? new User() };
            UserService service = new UserService();

            try
            {
                model.VipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);

                if (model.VipInfo != null)
                {
                    if (!model.VipInfo.State.HasValue || model.VipInfo.State.Value == (int)VipState.Normal || model.VipInfo.State.Value == (int)VipState.Invalid)
                    {
                        return Redirect(GetUrl("/user/Identify"));
                    }
                    else if (model.VipInfo.State.Value == (int)VipState.VIP)
                    {
                        return Redirect(GetUrl("/user/VipInfo"));
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("VipOrder 出错了！", e.ToString());
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult VipOrder(string vipDuration)
        {
            string result = "生成VIP会员充值订单失败";

            if (UserInfo != null)
            {
                UserService userService = new UserService();

                try
                {
                    if (UserInfo != null)
                    {
                        int duration = -1;
                        decimal totalAmount = 0;

                        int.TryParse(vipDuration, out duration);

                        if (duration < 1 || duration > 5)
                        {
                            throw new ArgumentException("充值VIP时间不正确，请重新选择");
                        }

                        totalAmount = 100 * duration;

                        UserVip vipInfo = userService.GetUserVipInfoByUserId(UserInfo.UserId);

                        if (vipInfo != null)
                        {
                            if (vipInfo.State == (int)VipState.Normal || vipInfo.State == (int)VipState.Invalid)
                            {
                                throw new ArgumentException("认证资料还没有通过审核，请先上传认证资料");
                            }

                            TradeOrder order = null;
                            string url = GetUrl("/user/VipInfo");

                            if (!string.IsNullOrEmpty(vipInfo.OrderId))
                            {
                                order = orderService.GetOrderByOrderId(vipInfo.OrderId);
                            }
                            if (order != null && order.UserName == UserInfo.UserName && order.Amount == totalAmount)
                            {
                                result = string.Format(Constant.PostPayInfoFormat, order.OrderId, url);
                            }
                            else
                            {
                                // 删掉原来的订单
                                if (order != null)
                                {
                                    orderService.DeleteOrderById(order.OrderId);
                                }
                                string orderId = orderService.GenerateNewOrderNumber();
                                string subject = "活动在线网 | 用户VIP会员充值";
                                string body = "用户" + UserInfo.UserName + "充值VIP会员" + duration + "年";
                                int userId = UserInfo.UserId;
                                string username = UserInfo.UserName;
                                decimal amount = totalAmount;
                                int state = (int)OrderState.New;
                                string resourceUrl = url;

                                order = orderService.AddNewOrder(orderId, subject, body, amount, state, username, resourceUrl, (int)OrderType.ToVip, userId);
                                bool success = userService.UpdateUserVipInfo(vipInfo.Id, orderId, vipInfo.IdentifyImg, vipInfo.StartTime, vipInfo.EndTime, duration, totalAmount, VipState.Identified);

                                if (success)
                                {
                                    result = string.Format(Constant.PostPayInfoFormat, orderId, url);
                                }
                                else
                                {
                                    result = "生成VIP充值支付订单信息失败，请重新尝试";
                                }
                            }
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    result = e.Message;
                }
                catch (Exception e)
                {
                    LogService.Log("生成VIP充值支付订单信息", e.ToString());
                    result = "生成VIP充值支付订单信息，请重新尝试";
                }
            }
            else
            {
                result = "您还未登录或登录超时，请重新登录";
            }

            return Content(result);
        }

        public ActionResult VipInfo()
        {
            RequireLogin();
            UserViewModel model = new UserViewModel { UserEntity = UserInfo ?? new User() };
            UserService service = new UserService();

            try
            {
                model.VipInfo = service.GetUserVipInfoByUserId(UserInfo.UserId);

                if (model.VipInfo != null)
                {
                    if (!model.VipInfo.State.HasValue || model.VipInfo.State.Value != (int)VipState.VIP)
                    {
                        return Redirect(GetUrl("/user/ToVip"));
                    }
                    else
                    {
                        if (model.VipInfo.EndTime > DateTime.Now)
                        {
                            model.ErrorMsg = "您的VIP已过期，请重新申请";
                            //service.SetUserToVip(
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("VipInfo 出错了！", e.ToString());
            }

            return View(model);
        }

        #endregion

        public ActionResult UpdatePassword()
        {
            RequireLogin();
            UserViewModel model = new UserViewModel { UserEntity = UserInfo };
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdatePassword(string oldpassword, string newpassword)
        {
            RequireLogin();
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
                    service.GetBackPasswordByCellphone(model.UserEntity);
                }
                else
                {
                    message = "原密码输入错误！";
                }
            }
            catch (Exception e)
            {
                LogService.Log("UpdatePassword 出错了！", e.ToString());
                message = "密码更新失败！";
            }
            model.ErrorMsg = message;
            return View(model);
        }

        #endregion

        #region 我的活动在线

        public ActionResult Activitys(int page = 1)
        {
            if (!IsUserLogin)
            {
                return Redirect("/account/login");
            }
            ActivitysViewModel model = new ActivitysViewModel();
            try
            {
                ActivityService service = new ActivityService();
                QueryActivityCriteria queryActivityCriteria = new QueryActivityCriteria
                {
                    UserId = UserInfo.UserId,
                    PageSize = 10,
                    StartRowIndex = page,
                    QueryType = 3
                };
                model.ActivityList = service.QueryActivities(queryActivityCriteria);
                if (model.ActivityList != null)
                {
                    model.TotalCount =
                        queryActivityCriteria.ResultTotalCount;
                }
                model.PageSize = 10;
                model.PageIndex = page;
                model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            }
            catch (Exception e)
            {
                LogService.Log("Activitys List 出错了！", e.ToString());
            }
            return View(model);
        }

        public ActionResult ShareActivity()
        {
            ShareActivityViewModel model = new ShareActivityViewModel();
            if (!IsIdentified)
            {
                return Redirect("/account/login");
                //model.ErrorMsg = "对不起，只有认证会员可以发布活动在线！";
                //model.ErrorCode = "401";
            }
            model.Provinces = (new CityService()).GetProvinces(true);
            model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ShareActivity(Activity activity)
        {
            ShareActivityViewModel model = new ShareActivityViewModel();
            if (!IsIdentified)
            {
                model.ErrorMsg = "对不起，只有认证会员才可以发布活动在线！";
                model.ErrorCode = "401";
            }
            else
            {
                try
                {
                    ActivityService service = new ActivityService();
                    activity.UserId = UserInfo.UserId;
                    activity.LocationId = Request.Form["LocationId[]"];
                    activity.State = 3;
                    if (!string.IsNullOrEmpty(activity.ContentText))
                    {
                        var clearText = HtmlUtil.RemoveHTMLTags(activity.ContentText);
                        activity.Description = clearText.Length >= 50
                            ? clearText.Substring(0, 50)
                            : clearText;
                    }
                    service.CreateOrUpdateActivity(activity);
                    model.ErrorMsg = "发布成功！";
                    model.ErrorCode = "200";
                    model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
                }
                catch (Exception e)
                {
                    LogService.Log("ShareActivity 出错了！", e.ToString());
                }
            }
            model.Activity = activity;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditActivity(int id)
        {
            ShareActivityViewModel model = new ShareActivityViewModel();
            if (!IsIdentified)
            {
                model.ErrorMsg = "对不起，只有VIP会员才可以编辑活动在线！";
                model.ErrorCode = "401";
            }
            else
            {
                ActivityService activityService = new ActivityService();

                model.Activity = activityService.GetActivityById(id);
                if (model.Activity != null &&
                    string.Equals(model.Activity.UserId, UserInfo.UserId))
                {
                    CityService cityService = new CityService();
                    model.Provinces = cityService.GetProvinces(true);
                    model.Cities = cityService.GetCitiesByProvinceId(model.Activity.Province, true);//市
                    model.Areas = cityService.GetAreasByCityId(model.Activity.City, true);//区

                    model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
                }
                else
                {
                    model.ErrorMsg = "对不起，未找到对应活动！";
                    model.ErrorCode = "404";
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditActivity(Activity activity)
        {
            ShareActivityViewModel model = new ShareActivityViewModel();
            if (!IsIdentified)
            {
                model.ErrorMsg = "对不起，只有Vip会员可以编辑活动在线！";
                model.ErrorCode = "401";
            }
            else
            {
                try
                {
                    ActivityService service = new ActivityService();
                    activity.UserId = UserInfo.UserId;
                    activity.LocationId = Request.Form["LocationId[]"];
                    if (!string.IsNullOrEmpty(activity.ContentText))
                    {
                        var clearText = HtmlUtil.RemoveHTMLTags(activity.ContentText);
                        activity.Description = clearText.Length >= 50
                            ? clearText.Substring(0, 50)
                            : clearText;
                    }
                    activity.State = 3;
                    service.CreateOrUpdateActivity(activity);
                    model.ErrorMsg = "修改成功！";
                    model.ErrorCode = "200";
                    model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
                }
                catch (Exception e)
                {
                    LogService.Log("EditActivity 出错了！", e.ToString());
                }
            }
            model.Activity = activity;
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteActivity(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                if (IsIdentified)
                {
                    ActivityService service = new ActivityService();
                    service.UpdateActivityStatu(new Activity
                    {
                        Id = id,
                        State = 1
                    });
                }
            }
            catch (Exception e)
            {
                LogService.Log("DeleteActivity 出错了！", e.ToString());
            }
            return Json(result);
        }

        #endregion

        public PartialViewResult UserMenu()
        {
            RequireLogin();
            UserInfo.IsVip = IsVip;
            UserInfo.IsIdentified = IsIdentified;
            return PartialView(UserInfo ?? new Model.User());
        }

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
                        errorMsg = "认证资料已通过审核，您现在是认证会员，还可以升级为VIP会员";
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
    }
}
