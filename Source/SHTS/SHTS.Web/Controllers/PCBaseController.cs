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
            if (!filterContext.HttpContext.Request.Url.OriginalString.ToLower().Contains("/content") &&
                !filterContext.HttpContext.Request.Url.OriginalString.ToLower().Contains("/common") &&
                !filterContext.HttpContext.Request.Url.OriginalString.ToLower().Contains("/city") &&
                !filterContext.HttpContext.Request.Url.OriginalString.ToLower().Contains("/verifyusername"))
            {
                if (IsAccessFromWechatDevice(filterContext.HttpContext.Request))
                {
                    var mobileUrl = GetWechatUrlString(filterContext.HttpContext.Request);
                    LogService.Log("OrginalUrl", filterContext.HttpContext.Request.Url.OriginalString);
                    LogService.Log("WechatUrl", mobileUrl);
                    filterContext.Result = new RedirectResult(mobileUrl);
                }
                else if (IsAccessFromMobileDevice(filterContext.HttpContext.Request))
                {
                    var mobileUrl = GetMobileUrlString(filterContext.HttpContext.Request);
                    LogService.Log("OrginalUrl", filterContext.HttpContext.Request.Url.OriginalString);
                    LogService.Log("MobileUrl", mobileUrl);
                    filterContext.Result = new RedirectResult(mobileUrl);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 如果是移动端访问，跳转到移动端页面
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private string GetMobileUrlString(HttpRequestBase request)
        {
            var originalUrl = request.Url.OriginalString;
            var host = request.Url.Host;
            var port = request.Url.Port;
            var mobileDomain = string.Empty;
            var pcDomain = string.Empty;

            if (port == 80)
            {
                mobileDomain = host + "/mobile";
                pcDomain = host;
            }
            else
            {
                mobileDomain = host + ":" + port + "/mobile";
                pcDomain = host + ":" + port;
            }

            if (port == 80)
            {
                string portInUrl = ":80";
                int portIndex = originalUrl.IndexOf(portInUrl);

                if (portIndex != -1)
                {
                    originalUrl = originalUrl.Remove(portIndex, portInUrl.Length);
                }
            }

            return originalUrl.Replace(pcDomain, mobileDomain);
        }

        /// <summary>
        /// 如果是微信端访问，跳转到微信端页面
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private string GetWechatUrlString(HttpRequestBase request)
        {
            var originalUrl = request.Url.OriginalString;
            var host = request.Url.Host;
            var port = request.Url.Port;
            var wechatDomain = string.Empty;
            var pcDomain = string.Empty;

            if (port == 80)
            {
                wechatDomain = host + "/wechat";
                pcDomain = host;
            }
            else
            {
                wechatDomain = host + ":" + port + "/wechat";
                pcDomain = host + ":" + port;
            }

            if (port == 80)
            {
                string portInUrl = ":80";
                int portIndex = originalUrl.IndexOf(portInUrl);

                if (portIndex != -1)
                {
                    originalUrl = originalUrl.Remove(portIndex, portInUrl.Length);
                }
            }

            return originalUrl.Replace(pcDomain, wechatDomain);
        }
    }
}
