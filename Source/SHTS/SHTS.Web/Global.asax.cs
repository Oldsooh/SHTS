using Senparc.Weixin.MP.TenPayLib;
using Senparc.Weixin.MP.TenPayLibV3;
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
                (PaymentService.ALIPAYSERVICE, "2088711708147372", "2i1dmrmyty1nnebyfmu8mmy41awfezmn", "utf-8", "http://www.huodongzaixian.com/pay/alipayresult", "http://www.huodongzaixian.com/pay/alipaynotify"));
            // Chinabank service, 取消网银在线支付.
            //HttpContext.Current.Cache.Insert(PaymentService.CHINABANKSERVICE, PayFactory.Create
            //    (PaymentService.CHINABANKSERVICE, "23133137", "shtscs123456789", "utf-8", "http://www.huodongzaixian.com/pay/chinabankresult", "http://www.huodongzaixian.com/pay/chinabanknotify"));

            //提供微信支付信息
            var weixinPay_PartnerId = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_PartnerId"];
            var weixinPay_Key = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_Key"];
            var weixinPay_AppId = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_AppId"];
            var weixinPay_AppKey = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_AppKey"];
            var weixinPay_TenpayNotify = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_TenpayNotify"];

            var tenPayV3_MchId = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"];
            var tenPayV3_Key = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_Key"];
            var tenPayV3_AppId = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_AppId"];
            var tenPayV3_AppSecret = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_AppSecret"];
            var tenPayV3_TenpayNotify = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_TenpayNotify"];

            var weixinPayInfo = new TenPayInfo(weixinPay_PartnerId, weixinPay_Key, weixinPay_AppId, weixinPay_AppKey, weixinPay_TenpayNotify);
            TenPayInfoCollection.Register(weixinPayInfo);
            var tenPayV3Info = new TenPayV3Info(tenPayV3_AppId, tenPayV3_AppSecret, tenPayV3_MchId, tenPayV3_Key,
                                                tenPayV3_TenpayNotify);
            TenPayV3InfoCollection.Register(tenPayV3Info);

            // Start wechat push subscribe demand working thread.
            //Subscription.WorkingThread.Instance.Run();
        }

        /// <summary>
        /// 处理未知的异常 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            //LogService.Log("程序发生了未知异常", ex.ToString());
            HttpException error = ex as HttpException;
            if (error != null)
            {
                if (error.GetHttpCode() != 404)
                {
                    Response.Redirect("/common/Error");
                }
                else
                {
                    Response.Redirect("/common/ErrorPageNotFound");
                }
            }

            Server.ClearError();
        }
    }
}