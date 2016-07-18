using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;
using Witbird.SHTS.Web.Areas.Wechat.Models;
using Witbird.SHTS.Web.Models;
using WitBird.Com.SearchEngine;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class IndexController : WechatBaseController
    {
        #region 搜索

        public const string IndexPath = "~/IndexData";
        public const string DictPath = @"~/Config/PanGu.xml";
        public const string HSplit = "|";
        public const string split = ",";

        #endregion

        SinglePageService singlePageService = new SinglePageService();
        DemandService demandService = new DemandService();
        TradeService tradeService = new TradeService();
        private ActivityService activityService;

        public ActionResult Index()
        {
            var model = new Witbird.SHTS.Web.Areas.Wechat.Models.HomeModel();

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
                LogService.Log("微信版首页", e.ToString());
            }

            if (model.Right == null)
            {
                model.Right = new Right();
            }

            return View(model);
        }

        public ActionResult Search(string keyWords, int page = 1)
        {
            SearchViewModel viewModel = new SearchViewModel();
            try
            {
                int totalHit = 0;
                IndexManager indexManager = new IndexManager(Server.MapPath(IndexPath), Server.MapPath(DictPath));

                viewModel.resultList = indexManager.SearchFromIndexDataByPage(keyWords, page, 15, out totalHit);
                viewModel.TotalHit = totalHit;

                viewModel.AllCount = totalHit;
                viewModel.PageIndex = page;
                viewModel.PageStep = 5;
                viewModel.PageSize = 15;
                viewModel.Keywords = keyWords;
                if (viewModel.AllCount % viewModel.PageSize == 0)
                {
                    viewModel.PageCount = viewModel.AllCount / viewModel.PageSize;
                }
                else
                {
                    viewModel.PageCount = viewModel.AllCount / viewModel.PageSize + 1;
                }
            }
            catch (Exception e)
            {
                LogService.Log("搜索失败---" + e.Message, e.ToString());
            }

            return View(viewModel);
        }
    }
}
