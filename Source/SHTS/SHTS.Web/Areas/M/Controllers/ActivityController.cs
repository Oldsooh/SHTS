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
using Witbird.SHTS.Web.Controllers;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.ActivityModel;

namespace Witbird.SHTS.Web.Areas.M.Controllers
{
    public class ActivityController : MobileBaseController
    {
        //
        // GET: /M/Activity/

        public ActionResult Index(int page = 1)
        {
            ActivitysViewModel model = new ActivitysViewModel();
            try
            {
                ActivityService service = new ActivityService();
                QueryActivityCriteria queryActivityCriteria = new QueryActivityCriteria
                {
                    PageSize = 20,
                    StartRowIndex = page,
                    QueryType = -1
                };
                model.ActivityList = service.QueryActivities(queryActivityCriteria);

                if (model.ActivityList != null)
                {
                    model.TotalCount =
                        queryActivityCriteria.ResultTotalCount;
                }
                model.PageSize = 20;
                model.CurrentPage = page;
                model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            }
            catch (Exception e)
            {
                LogService.Log("Activitys List 出错了！", e.ToString());
            }
            return View(model);
        }

        public ActionResult Show(int id)
        {
            ShowActivityViewModel model = new ShowActivityViewModel();
            try
            {
                ActivityService service = new ActivityService();
                model.Activity = service.GetActivityById(id);
                
                model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            }
            catch (Exception e)
            {
                LogService.Log("Show Activity出错了！", e.ToString());
            }
            return View(model);
        }

        public ActionResult ShareActivity()
        {
            ShareActivityViewModel model = new ShareActivityViewModel();
            RequireLogin();
            model.Provinces = (new CityService()).GetProvinces(true);
            model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ShareActivity(Activity activity)
        {
            ShareActivityViewModel model = new ShareActivityViewModel();
            model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            if (!IsUserLogin)
            {
                model.ErrorMsg = "未登录或登录超时";
                model.ErrorCode = "401";
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(activity.Title) ||
                        string.IsNullOrEmpty(activity.Description) || 
                        string.IsNullOrEmpty(activity.Adress))
                    {
                        model.ErrorMsg = "标题或内容不能为空";
                        model.ErrorCode = "400";
                    }
                    else
                    {
                        ActivityService service = new ActivityService();
                        activity.UserId = UserInfo.UserId;
                        activity.LocationId = Request.Form["LocationId[]"];
                        activity.State = 3;
                        activity.IsFromMobile = true;
                        activity.Description = Witbird.SHTS.Web.Public.StaticUtility.FilterSensitivewords(activity.Description);
                        activity.ContentStyle = activity.Description;
                        activity.ContentText = activity.Description;
                        service.CreateOrUpdateActivity(activity);
                        model.ErrorMsg = "发布成功！";
                        model.ErrorCode = "200";
                        return RedirectToAction("Index", "Activity", new { Area = "M" });
                    }
                }
                catch (Exception e)
                {
                    LogService.Log("ShareActivity 出错了！", e.ToString());
                }
            }
            model.Activity = activity;
            return View(model);
        }
    }
}
