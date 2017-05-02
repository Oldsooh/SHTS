using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AccessRecord : IHttpHandler, IRequiresSessionState
    {
        private const string Content_Type = "text/plain";
        private const string PageType = "PageType";
        private const string PageId = "PageId";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = Content_Type;
            var type = context.Request.QueryString[PageType];
            var id = context.Request.QueryString[PageId];
            var accessUrl = context.Request.UrlReferrer == null ? null
                : context.Request.UrlReferrer.ToString();

            if (string.IsNullOrWhiteSpace(type) ||
                string.IsNullOrWhiteSpace(id) ||
                string.IsNullOrWhiteSpace(accessUrl))
            {
                return;
            }

            var userIP = context.Request.UserHostAddress;
            var access = new Model.AccessRecord
            {
                AccessUrl = accessUrl,
                InsertedTimestamp = DateTime.Now,
                UserIP = userIP
            };
            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                try
                {
                    var tableName = "";
                    var primaryId = "";
                    var primaryValue = "";
                    var columnName = "";

                    switch(type.ToLower())
                    {
                        case "resource":
                            tableName = "Resource";
                            primaryId = "Id";
                            primaryValue = id;
                            columnName = "ReadCount";
                            break;
                        case "demand":
                        case "activity":
                        case "singlepage":
                            tableName = type;
                            primaryId = "Id";
                            primaryValue = id;
                            columnName = "ViewCount";
                            break;
                        case "trade":
                            tableName = type;
                            primaryId = "TradeId";
                            primaryValue = id;
                            columnName = "ViewCount";
                            break;
                    }
                    AccessRecordManager.Instance.Record(state as Model.AccessRecord, tableName, primaryId, primaryValue, columnName);
                }
                catch (Exception exception)
                {
                    LogService.Log("记录用户访问次数失败", exception.ToString());
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