using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 会员管理
    /// </summary>
    [Permission(EnumRole.Normal, EnumBusinessPermission.AccountManage_User)]
    public class MemberController : AdminBaseController
    {
        public ActionResult Index(int querytype = 0, int page = 1, string Keyword = "")
        {
            MemberViewModel viewModel = new MemberViewModel();
            try
            {
                UserService service = new UserService();
                int totalCount;
                int pageSize = 20;
                viewModel.UserList = service.QuersyUsers("-1", -1, page, pageSize,
                    querytype, Keyword, out totalCount);
                viewModel.PageSize = pageSize;
                viewModel.PageStep = 5;
                viewModel.CurrentPage = page;
                viewModel.Keyword = Keyword;
                viewModel.QueryType = querytype;
                viewModel.TotalCount = totalCount;
            }
            catch (Exception ex)
            {
                LogService.Log("会员列表", ex.ToString());
            }
            return View(viewModel);
        }

        public ActionResult EditUser(int id)
        {
            MemberViewModel viewModel = new MemberViewModel();
            try
            {
                UserService service = new UserService();
                viewModel.SingleUser = service.GetUserById(id);
            }
            catch (Exception ex)
            {
                LogService.Log("会员列表", ex.ToString());
            }
            return View(viewModel);
        }

        [Permission(EnumRole.Editer, EnumBusinessPermission.MemberManage_UpdateMemberInfo)]
        [HttpPost]
        public ActionResult EditUser(string UserId, string Cellphone, string Email,
            string QQ, string Address, string UCard, string SiteUrl)
        {
            MemberViewModel viewModel = new MemberViewModel();
            try
            {
                UserService service = new UserService();
                User user = service.GetUserById(int.Parse(UserId));

                if (user != null)
                {
                    user.Cellphone = Cellphone;
                    user.Email = Email;
                    user.QQ = QQ;
                    user.Adress = Address;
                    user.UCard = UCard;
                    user.SiteUrl = SiteUrl;
                }

                service.UserUpdate(user);
                viewModel.SingleUser = service.GetUserById(user.UserId); ;
            }
            catch (Exception ex)
            {
                LogService.Log("会员列表", ex.ToString());
            }
            return View(viewModel);
        }

        [Permission(EnumRole.SystemAdmin, EnumBusinessPermission.MemberManage_DeleteMember)]
        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                UserService service = new UserService();
                if (!service.DeleteUser(id))
                {
                    result.ExceptionInfo = "删除失败";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("DeleteUser", ex.ToString());
                result.ExceptionInfo = "删除失败";
            }
            return Json(result);
        }

        /// <summary>
        /// 会员统计
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="querytype"></param>
        /// <returns></returns>
        public ActionResult TongJi(string city = "-1", int resurce = -1, 
            int querytype = -1, int page = 1, string Keyword="")
        {
            MemberViewModel viewModel = new MemberViewModel();
            try
            {
                UserService service = new UserService();
                int totalCount;
                viewModel.UserList = service.QuersyUsersByCityAndResource(city, resurce, page, 20,
                    querytype, Keyword, out totalCount);
                viewModel.PageSize = 20;
                viewModel.PageStep = 5;
                viewModel.CurrentPage = page;
                viewModel.TotalCount = totalCount;
                viewModel.Resurce = resurce;
                viewModel.City = city;
                viewModel.QueryType = querytype;
                viewModel.Keyword = Keyword;
            }
            catch (Exception ex)
            {
                LogService.Log("会员列表", ex.ToString());
            }
            return View(viewModel);
        }

        public ActionResult VipUserReview(int page = 1, int size = 10)
        {
            MemberViewModel viewModel = new MemberViewModel();
            try
            {
                UserService service = new UserService();
                int totalCount;
                viewModel.VipInfos = service.GetVipUserReviewList(page, size, out totalCount);
                viewModel.PageSize = size;
                viewModel.CurrentPage = page;
                viewModel.TotalCount = totalCount;
            }
            catch (Exception ex)
            {
                LogService.Log("VIP会员审核列表", ex.ToString());
            }
            return View(viewModel);
        }

        /// <summary>
        /// 会员认证审核不通过，删除会员认证信息，方法名有点不对，就不改了
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Permission(EnumRole.SystemAdmin)]
        [HttpPost]
        public JsonResult DeleteVipInfo(int id, int userId)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                UserService service = new UserService();
                if (!service.DeleteVipInfo(id, userId))
                {
                    result.ExceptionInfo = "删除失败";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("DeleteVipInfo", ex.ToString());
                result.ExceptionInfo = "删除失败";
            }
            return Json(result);
        }

        [Permission(EnumRole.SystemAdmin)]
        [HttpPost]
        public JsonResult ReviewedVipInfo(int vipId, int userId)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                UserService service = new UserService();
                if (!service.ReviewedVipInfo(vipId, userId))
                {
                    result.ExceptionInfo = "审核失败";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("ReviewedVipInfo", ex.ToString());
                result.ExceptionInfo = "审核失败";
            }
            return Json(result);
        }

        [Permission(EnumRole.SystemAdmin)]
        [HttpPost]
        public JsonResult SetUserVip(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                UserService service = new UserService();
                User user = service.GetUserById(id);

                if (user.Vip.HasValue && user.Vip == (int)VipState.VIP)
                {
                    result.ExceptionInfo = "该用户已经是VIP会员";
                }
                else if (user.Vip.HasValue && (user.Vip == (int)VipState.Normal || user.Vip == (int)VipState.Invalid))
                {
                    result.ExceptionInfo = "只有认证会员才能设置VIP，请先通知会员进行认证，保证信息准确";
                }
                else if (!service.SetUserToVip(id))
                {
                    result.ExceptionInfo = "用户升级1年VIP失败";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("SetUserVip", ex.ToString());
                result.ExceptionInfo = "用户升级1年VIP失败";
            }
            return Json(result);
        }

        [Permission(EnumRole.SystemAdmin)]
        [HttpPost]
        public JsonResult CancelIdentify(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                UserService service = new UserService();
                User user = service.GetUserById(id);

                if (user.Vip.HasValue && user.Vip == (int)VipState.VIP)
                {
                    result.ExceptionInfo = "该用户是VIP会员，不能取消认证。如确实需要，请先取消VIP会员之后重新尝试";
                }
                else if (!service.CancelIdentify(id))
                {
                    result.ExceptionInfo = "取消会员认证失败";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("CancelIdentify", ex.ToString());
                result.ExceptionInfo = "取消会员认证失败";
            }
            return Json(result);
        }

        [Permission(EnumRole.SystemAdmin)]
        [HttpPost]
        public JsonResult CancelUserVip(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                UserService service = new UserService();
                if (!service.CancelUserVip(id))
                {
                    result.ExceptionInfo = "取消用户VIP失败";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("CancelUserVip", ex.ToString());
                result.ExceptionInfo = "取消用户VIP失败";
            }
            return Json(result);
        }
    }
}
