using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web
{
    /// <summary>
    /// Analysis 的摘要说明
    /// </summary>
    public class Analysis : IHttpHandler, IRequiresSessionState
    {
        private const string title_para = "title";
        private const string title_operation = "operation";
        private const string title_from = "from";
        private const string Derivet = "直接链接访问";
        private const string BaiduSearch = "百度搜索";
        private const string Baidu = "baidu";

        private const string Content_Type = "text/plain";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = Content_Type;
            var referrerUrl = context.Request.QueryString[title_from];
            var accessUrl = context.Request.UrlReferrer == null ? null
                : context.Request.UrlReferrer.ToString();
            var IP = context.Request.UserHostAddress;
            var title = context.Request.QueryString[title_para];
            var operation = context.Request.QueryString[title_operation];
            var access = new AccessAnalytics
            {
                ReferrerUrl = referrerUrl,
                AccessUrl = accessUrl,
                IP = IP,
                PageTitle = title,
                Operation = operation
            };
            if (string.IsNullOrEmpty(referrerUrl))
            {
                access.ReferrerUrl = Derivet;
            }
            else
            {
                if (referrerUrl.Contains(Baidu))
                {
                    access.ReferrerUrl = BaiduSearch;
                }
            }
            if (context.Session != null &&
                context.Session["userinfo"] != null)
            {
                access.UserId = ((User)context.Session["userinfo"]).UserId;
            }
            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                try
                {
                    AccessAnalyticsService.Instance.AccessTrack(state as AccessAnalytics);
                }
                catch (Exception exception)
                {
                    LogService.Log("记录用户行为失败", exception.ToString());
                }
            }, access);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}