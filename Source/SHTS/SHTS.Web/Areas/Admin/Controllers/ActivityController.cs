using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.ActivityModel;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    [Permission(EnumRole.Normal, EnumBusinessPermission.None)]
    public class ActivityController : AdminBaseController
    {

        public ActionResult Index(int page = 1, string Status="")
        {
            ActivitysViewModel model = new ActivitysViewModel();
            try
            {
                ActivityService service = new ActivityService();
                QueryActivityCriteria queryActivityCriteria = new QueryActivityCriteria
                {
                    PageSize = 35,
                    StartRowIndex = page,
                    QueryType = 5,
                    Status = string.IsNullOrEmpty(Status) ? "0,2,3,4":Status
                };
                model.ActivityList = service.QueryActivities(queryActivityCriteria);
                if (model.ActivityList != null)
                {
                    model.TotalCount =
                        queryActivityCriteria.ResultTotalCount;
                }
                model.PageSize = queryActivityCriteria.PageSize;
                model.PageIndex = queryActivityCriteria.StartRowIndex;
                model.PageStep = 5;
                model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            }
            catch (Exception e)
            {
                LogService.Log("Activitys List 出错了！", e.ToString());
            }
            ViewBag.Status = Status;
            return View(model);
        }

        public JsonResult UpdateStatus(int id,int sid)
        {
            AjaxResult result=new AjaxResult();
            try
            {
                ActivityService service = new ActivityService();
                service.UpdateActivityStatu(new Activity
                {
                    Id=id,
                    State = sid
                });
            }
            catch (Exception e)
            {
                LogService.Log("Update Status Activitys 出错了！", e.ToString());
                result.ExceptionInfo = "出错了";
                result.ErrorCode = 102;
            }
            return Json(result);
        }

        public ActionResult TongJi(int page = 1, string Keyword = "",
            string StartTime = "", string EndTime = "")
        {
            ActivitysViewModel model = new ActivitysViewModel();
            try
            {
                ActivityService service = new ActivityService();
                QueryActivityCriteria queryActivityCriteria = new QueryActivityCriteria
                {
                    PageSize = 35,
                    StartRowIndex = page,
                    QueryType = 7,
                    Keyword = Keyword,
                    StartTime = string.IsNullOrWhiteSpace(StartTime) ? new DateTime(2014,1,1) : Convert.ToDateTime(StartTime),
                    EndTime = string.IsNullOrWhiteSpace(EndTime) ? DateTime.Now : Convert.ToDateTime(EndTime)
                };
                model.ActivityList = service.QueryActivities(queryActivityCriteria);
                if (model.ActivityList != null)
                {
                    model.TotalCount =
                        queryActivityCriteria.ResultTotalCount;
                }
                model.StartTime = queryActivityCriteria.StartTime.Value;
                model.EndTime = queryActivityCriteria.EndTime.Value;
                model.PageSize = queryActivityCriteria.PageSize;
                model.PageIndex = queryActivityCriteria.StartRowIndex;
                model.PageStep = 5;
                model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            }
            catch (Exception e)
            {
                LogService.Log("Activitys TongJi 出错了！", e.ToString());
            }
            return View(model);
        }
    }
}
