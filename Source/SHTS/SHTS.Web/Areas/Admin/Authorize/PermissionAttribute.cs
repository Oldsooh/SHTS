using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Witbird.SHTS.Web.Areas.Admin.Authorize
{
    /// <summary>
    /// 用于权限点认证，标记在Action上面
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class PermissionAttribute : FilterAttribute, IActionFilter
    {
        public List<EnumBusinessPermission> Permissions { get; set; }

        public EnumRole Role { get; set; }

        public PermissionAttribute(params EnumBusinessPermission[] parameters)
        {
            this.Role = EnumRole.Normal;
            Permissions = parameters.ToList();
        }

        public PermissionAttribute(EnumRole role, params EnumBusinessPermission[] parameters)
        {
            this.Role = role;
            Permissions = parameters.ToList();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //throw new NotImplementedException();
        }
    }

}