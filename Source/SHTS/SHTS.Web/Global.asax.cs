using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Witbird.SHTS.Common;
using WitBird.Com.Pay;

namespace Witbird.SHTS.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ThirdPartyConfig.Config();

            // Initializes online payment service interface.
            // Alipay service.
            HttpContext.Current.Cache.Insert(PaymentService.ALIPAYSERVICE, PayFactory.Create
                (PaymentService.ALIPAYSERVICE, "2088711708147372", "2i1dmrmyty1nnebyfmu8mmy41awfezmn", "utf-8", "http://www.activity-line.com/pay/alipayresult", "http://www.activity-line.com/pay/alipaynotify"));
            // Chinabank service, 取消网银在线支付.
            //HttpContext.Current.Cache.Insert(PaymentService.CHINABANKSERVICE, PayFactory.Create
            //    (PaymentService.CHINABANKSERVICE, "23133137", "shtscs123456789", "utf-8", "http://www.activity-line.com/pay/chinabankresult", "http://www.activity-line.com/pay/chinabanknotify"));
        }

        /// <summary>
        /// 处理未知的异常 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            HttpException error = Server.GetLastError() as HttpException;
            if (error != null)
            {
                if (error.GetHttpCode() != 404)
                {
                    LogService.Log("程序发生了未知异常", error.ToString());
                    Server.ClearError();
                    Response.Redirect("/common/Error");
                    //Context.Handler = PageParser.GetCompiledPageInstance(path, Server.MapPath(path), Context); 
                }
            }
        }
    }
}