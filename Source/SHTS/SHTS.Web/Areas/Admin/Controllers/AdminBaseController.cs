using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {
        public const string USERINFO = "AdminUserInfo";

        #region 登陆用户信息

        public AdminUser UserInfo
        {
            get
            {
                return Session[USERINFO] as AdminUser;
            }
        }

        #endregion

        #region 通知前台方法

        /// <summary>
        /// 转向到一个提示页面，然后自动返回指定的页面
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="redirect"></param>
        /// <returns></returns>
        public ContentResult Stop(string notice, string redirect, bool isAlert = false)
        {
            var content = "<meta http-equiv='refresh' content='1;url=" + redirect + "' /><body style='margin-top:0px;color:red;font-size:24px;'>" + notice + "</body>";

            if (isAlert)
                content = string.Format("<script>alert('{0}'); window.location.href='{1}'</script>", notice, redirect);

            return this.Content(content);
        }

        #endregion

        #region override

        /// <summary>
        /// 方法执行前，如果没有登录就调整到Passport登录页面，没有权限就抛出信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var noAuthorizeAttributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeIgnoreAttribute), false);
            if (noAuthorizeAttributes.Length > 0)
                return;

            base.OnActionExecuting(filterContext);

            if (this.UserInfo == null)
            {
                filterContext.Result = RedirectToAction("Login", "Account", new { Area = "Admin" });
                return;
            }

            bool hasPermission = true;
            var permissionAttributes = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(PermissionAttribute), false).Cast<PermissionAttribute>();
            permissionAttributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(PermissionAttribute), false).Cast<PermissionAttribute>().Union(permissionAttributes);
            var attributes = permissionAttributes as IList<PermissionAttribute> ?? permissionAttributes.ToList();
            if (permissionAttributes != null && attributes.Count() > 0)
            {
                hasPermission = true;
                foreach (var attr in attributes)
                {
                    if (this.UserInfo.Role.HasValue && this.UserInfo.Role.Value > (int)attr.Role)
                    {
                        foreach (var permission in attr.Permissions)
                        {
                            //if (this.UserInfo.Role.HasValue && this.UserInfo.Role.Value >= (int)attr.Role)
                            //{
                            //    hasPermission = false;
                            //    break;
                            //}
                        }
                    }
                    else
                    {
                        hasPermission = false;
                        break;
                    }
                }

                if (!hasPermission)
                {
                    if (Request.UrlReferrer != null)
                        filterContext.Result = this.Stop("没有权限！", Request.UrlReferrer.AbsoluteUri);
                    else
                        filterContext.Result = Content("没有权限！");
                }
            }
        }

        /// <summary>
        /// 如果是Ajax请求的话，清除浏览器缓存
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetNoStore();
            }

            base.OnResultExecuted(filterContext);
        }

        /// <summary>
        /// 发生异常写Log
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            var e = filterContext.Exception;

            LogService.Log(e.Message, e.ToString().ToString());
        }

        #endregion

    }
}
