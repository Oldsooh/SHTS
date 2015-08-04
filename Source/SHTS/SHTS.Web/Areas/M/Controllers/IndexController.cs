using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;
using Witbird.SHTS.Web.Areas.M.Models;

namespace Witbird.SHTS.Web.Areas.M.Controllers
{
    public class IndexController : Controller
    {
        SinglePageService singlePageService = new SinglePageService();
        DemandService demandService = new DemandService();
        TradeService tradeService = new TradeService();
        private ActivityService activityService;

        public ActionResult Index()
        {
            HomeModel model = new HomeModel();

            try
            {
                int tempCount = 0;
                model.Demands = demandService.GetDemands(10, 1, out tempCount);
                //model.Trades = tradeService.GetTradeList(5, 1, -1, out tempCount);

                //活动
                activityService = new ActivityService();
                model.ActivityList = activityService.QueryActivities(new QueryActivityCriteria
                {
                    PageSize = 10,
                    StartRowIndex = 1,
                    QueryType = -1
                });

                model.Right = new Right();
                CommonService commonService = new CommonService();
                model.Right = commonService.GetRight();

            }
            catch (Exception e)
            {
                LogService.Log("移动版首页", e.ToString());
            }

            if (model.Right == null)
            {
                model.Right = new Right();
            }

            return View(model);
        }

    }
}
