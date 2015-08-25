using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model.Criteria;
using Witbird.SHTS.Web.Models.ActivityModel;

namespace Witbird.SHTS.Web.Controllers
{
    public class ActivityController : BaseController
    {
        //
        // GET: /Activity/

        public ActionResult Index(int page=1)
        {
            ActivitysViewModel model = new ActivitysViewModel();
            try
            {
                ActivityService service = new ActivityService();
                QueryActivityCriteria queryActivityCriteria = new QueryActivityCriteria
                {
                    PageSize = 35,
                    StartRowIndex = page,
                    QueryType = -1
                };
                model.ActivityList = service.QueryActivities(queryActivityCriteria);

                queryActivityCriteria.PageSize = 5;
                queryActivityCriteria.QueryType = 4;
                queryActivityCriteria.StartRowIndex = page;
                model.PreTypeActivityList = service.QueryActivities(queryActivityCriteria);

                if (model.ActivityList != null)
                {
                    model.TotalCount =
                        queryActivityCriteria.ResultTotalCount;
                }
                model.PageSize = 35;
                model.PageIndex = page;
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
                QueryActivityCriteria queryActivityCriteria = new QueryActivityCriteria
                {
                    PageSize = 6,
                    StartRowIndex = 1,
                    QueryType = -1
                };
                model.ActivityList = service.QueryActivities(queryActivityCriteria);

                queryActivityCriteria.PageSize = 8;
                queryActivityCriteria.QueryType = 4;
                queryActivityCriteria.StartRowIndex = 1;
                model.PreTypeActivityList = service.QueryActivities(queryActivityCriteria);

                
                model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            }
            catch (Exception e)
            {
                LogService.Log("Show Activity出错了！", e.ToString());
            }
            return View(model);
        }
    }
}
