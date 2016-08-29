using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Web.Areas.Wechat.Common;

namespace Witbird.SHTS.Web.Areas.Mobile.Controllers
{
    public class MobileBaseController : Witbird.SHTS.Web.Controllers.BaseController
    {
        /// <summary>
        /// 在执行具体Action之前进行微信权限检测，保存wechat用户信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SetDefaultCityToSession();
            base.OnActionExecuting(filterContext);            
        }
        
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Response.Redirect("/mobile/error/error_503");
        }
    }
}
