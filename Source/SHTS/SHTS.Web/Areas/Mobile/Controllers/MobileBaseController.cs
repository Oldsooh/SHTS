using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Common;

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
            // SetDefaultCityToSession();

            if (!filterContext.HttpContext.Request.Url.OriginalString.Contains("/content/") &&
                !filterContext.HttpContext.Request.Url.OriginalString.Contains("/common/") &&
                !filterContext.HttpContext.Request.Url.OriginalString.Contains("/city/"))
            {
                // 如果是wechat访问，则跳转到wechat页面
                if (IsAccessFromWechatDevice(filterContext.HttpContext.Request))
                {
                    var pcUrl = GetWechatUrlString(filterContext.HttpContext.Request);
                    LogService.Log("OrginalUrl", filterContext.HttpContext.Request.Url.OriginalString);
                    LogService.Log("WechatUrl", pcUrl);
                    filterContext.Result = new RedirectResult(pcUrl);
                }
                else if (IsAccessFromPCDevice(filterContext.HttpContext.Request))
                {
                    var pcUrl = GetPCUrlString(filterContext.HttpContext.Request);
                    LogService.Log("OrginalUrl", filterContext.HttpContext.Request.Url.OriginalString);
                    LogService.Log("PCUrl", pcUrl);
                    filterContext.Result = new RedirectResult(pcUrl);
                }
            }

            //if (string.IsNullOrEmpty(CurrentCityId))
            //{
            //    filterContext.Result = new RedirectResult("/mobile/city/index?returnUrl=" + filterContext.HttpContext.Request.Url.AbsoluteUri);
            //}

            base.OnActionExecuting(filterContext);            
        }
        
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Response.Redirect("/mobile/error/error_503");
        }

        /// <summary>
        /// 如果不是移动端访问，跳转到PC端页面
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private string GetPCUrlString(HttpRequestBase request)
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

            return originalUrl.Replace(mobileDomain, pcDomain);
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
            var mobileDomain = string.Empty;

            if (port == 80)
            {
                wechatDomain = host + "/wechat";
                mobileDomain = host + "/mobile";
            }
            else
            {
                wechatDomain = host + ":" + port + "/wechat";
                mobileDomain = host + ":" + port + "/mobile";
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

            return originalUrl.Replace(mobileDomain, wechatDomain);
        }
    }
}
