using System;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Witbird.SHTS.BLL.Managers;
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
        private const string PageType = "PageType";
        private const string PageId = "PageId";

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
            var pageType = context.Request.QueryString[PageType];
            var pageId = context.Request.QueryString[PageId];

            #region 根据IP增加阅读数量
            if (!string.IsNullOrWhiteSpace(pageType) &&
                !string.IsNullOrWhiteSpace(pageId) &&
                !string.IsNullOrWhiteSpace(accessUrl))
            {
                var accessRecord = new Model.AccessRecord
                {
                    UserIP = IP,
                    AccessUrl = accessUrl,
                    InsertedTimestamp = DateTime.Now
                };
                ThreadPool.QueueUserWorkItem(delegate (object state)
                {
                    try
                    {
                        var tableName = "";
                        var primaryId = "";
                        var primaryValue = "";
                        var columnName = "";

                        switch (pageType.ToLower())
                        {
                            case "resource":
                                tableName = "Resource";
                                primaryId = "Id";
                                primaryValue = pageId;
                                columnName = "ReadCount";
                                break;
                            case "demand":
                            case "activity":
                            case "singlepage":
                                tableName = pageType;
                                primaryId = "Id";
                                primaryValue = pageId;
                                columnName = "ViewCount";
                                break;
                            case "trade":
                                tableName = pageType;
                                primaryId = "TradeId";
                                primaryValue = pageId;
                                columnName = "ViewCount";
                                break;
                        }
                        AccessRecordManager.Instance.Record(state as Model.AccessRecord, tableName, primaryId, primaryValue, columnName);
                    }
                    catch (Exception exception)
                    {
                        LogService.Log("记录用户访问次数失败", exception.ToString());
                    }
                }, accessRecord);
            }

            #endregion

            #region 用户访问记录
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

            #endregion
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