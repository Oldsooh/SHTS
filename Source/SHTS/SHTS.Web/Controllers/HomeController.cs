using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Controllers
{
    public class HomeController : PCBaseController
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
                model.Newses = singlePageService.GetSinglePages(EnumEntityType.News.ToString(), null, true, 5, 1, out tempCount);
                model.Demands = demandService.GetDemands(20, 1, out tempCount);
                model.Trades = tradeService.GetTradeList(5, 1, -1, out tempCount);

                //活动
                activityService = new ActivityService();
                model.ActivityList = activityService.QueryActivities(new QueryActivityCriteria
                {
                    PageSize = 10,
                    StartRowIndex = 1,
                    QueryType = -1
                });

            }
            catch (Exception e)
            {
                LogService.Log("首页", e.ToString());
            }

            if (model.Newses == null)
            {
                model.Newses = new List<SinglePage>();
            }

            return View(model);
        }
    }
}
