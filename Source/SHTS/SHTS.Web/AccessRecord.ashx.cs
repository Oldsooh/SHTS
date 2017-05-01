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
        private const string Type = "Type";
        private const string Id = "Id";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = Content_Type;
            var type = context.Request.QueryString[Type];
            var id = context.Request.QueryString[Id];
            var accessUrl = context.Request.UrlReferrer == null ? null
                : context.Request.UrlReferrer.ToString();
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
                            columnName = "ClickCount";
                            break;
                        case "demand":
                            tableName = "Demand";
                            primaryId = "Id";
                            primaryValue = id;
                            columnName = "ViewCount";
                            break;
                        case "activity":
                            tableName = "Activity";
                            primaryId = "Id";
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