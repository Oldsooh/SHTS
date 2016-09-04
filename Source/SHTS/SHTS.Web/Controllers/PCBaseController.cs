using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Common;

namespace Witbird.SHTS.Web.Controllers
{
    public abstract class PCBaseController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Browser.IsMobileDevice)
            {
                var mobileUrl = GetMobileUrlString(filterContext);
                LogService.Log("OrginalUrl", filterContext.HttpContext.Request.Url.OriginalString);
                LogService.Log("MobileUrl", mobileUrl);
                filterContext.Result = new RedirectResult(mobileUrl);
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 如果是移动端访问，跳转到移动端页面
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private string GetMobileUrlString(ActionExecutingContext filterContext)
        {
            var originalUrl = filterContext.HttpContext.Request.Url.OriginalString;
            var domain = filterContext.HttpContext.Request.Url.Host;

            if (originalUrl.IndexOf(":80") != -1)
            {
                originalUrl = originalUrl.Remove(originalUrl.IndexOf(":80"), ":80".Length);
            }

            var mobileDomain = domain + "/mobile";
            return originalUrl.Replace(domain, mobileDomain);
        }
    }
}
