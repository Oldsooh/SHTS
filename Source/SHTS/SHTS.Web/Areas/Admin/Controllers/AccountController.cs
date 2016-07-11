using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class AccountController : AdminBaseController
    {
        [AuthorizeIgnore]
        public ActionResult Login()
        {
            ViewBag.Message = string.Empty;
            return View();
        }

        [AuthorizeIgnore]
        public ActionResult Logout()
        {
            Session[USERINFO] = null;
            return RedirectToAction("Login"); ;
        }

        [AuthorizeIgnore]
        [HttpPost]
        public ActionResult Login(string username,string password)
        {
            var errorMsg = string.Empty;
            try
            {
                AdminUser result = null;

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    AdminUserManager adminManager = new AdminUserManager();
                    result = adminManager.Login(username, password);
                    if (result != null)
                    {
                        Session[USERINFO] = result;
                        return Redirect("/admin/");
                    }
                    else
                    {
                        errorMsg = "用户名或密码错误！";
                    }
                }
                else
                {
                    errorMsg = "用户名密码不可为空！";
                }
            }
            catch (Exception ex)
            {
                LogService.Log("注册用户", ex.ToString());
            }
            ViewBag.Message = errorMsg;
            return View();
        }

        [Permission(EnumRole.Normal)]
        public ActionResult Index()
        {
            AdminUserViewModel viewModel = new AdminUserViewModel();
            try
            {
                AdminRoleManager roleManager = new AdminRoleManager();
                viewModel.AllRoles = roleManager.GetAllRoles();
                AdminUserManager adminManager = new AdminUserManager();
                viewModel.AllAdminUsers = adminManager.GetALl();
            }
            catch (Exception ex)
            {
                LogService.Log("管理员列表", ex.ToString());
            }
            return View(viewModel);
        }

        [Permission(EnumRole.SystemAdmin,EnumBusinessPermission.AccountManage_User)]
        [HttpPost]
        public ActionResult AddNewAdmin(AdminUser admin)
        {
            try
            {
                if (admin != null && admin.Role != 0)
                {
                    AdminUserManager adminManager = new AdminUserManager();
                    admin.CreateTime = DateTime.Now;
                    admin.LastUpdatedTime = DateTime.Now;
                    admin.EncryptedPassword = admin.EncryptedPassword.ToMD5();
                    adminManager.AddAdminUser(admin);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("AddNewAdmin", ex.ToString());
            }
            return RedirectToAction("Index");
        }

        [Permission(EnumRole.SystemAdmin, EnumBusinessPermission.AccountManage_User)]
        [HttpPost]
        public ActionResult UpdateAdmin(AdminUser admin)
        {
            try
            {
                if (admin != null && admin.AdminId != 0)
                {
                    AdminUserManager adminManager = new AdminUserManager();
                    admin.EncryptedPassword = admin.EncryptedPassword.ToMD5();
                    adminManager.UpdateAdminUser(admin);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("UpdateAdmin", ex.ToString());
            }
            return RedirectToAction("Index");
        }

        [Permission(EnumRole.SystemAdmin, EnumBusinessPermission.AccountManage_User)]
        [HttpPost]
        public JsonResult DeleteResult(string ids)
        {
            AjaxResult result=new AjaxResult();
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    AdminUserManager adminManager = new AdminUserManager();
                    string[] uIds = ids.Split(',').ToArray();
                    foreach (var id in uIds)
                    {
                        adminManager.deleteAdminUser(id.ToInt32());
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("DeleteResult", ex.ToString());
            }
            return Json(result);
        }
    }
}
