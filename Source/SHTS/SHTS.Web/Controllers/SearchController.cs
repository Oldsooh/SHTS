using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Common;
using Witbird.SHTS.Web.Models;
using WitBird.Com.SearchEngine;

namespace Witbird.SHTS.Web.Controllers
{
    public class SearchController : Controller
    {
        #region 搜索

        public const string IndexPath = "~/IndexData";
        public const string DictPath = @"~/Config/PanGu.xml";
        public const string HSplit = "|";
        public const string split = ",";

        #endregion

        public ActionResult Index(string keyWords, int page = 1)
        {
            SearchViewModel viewModel=new SearchViewModel();
            try
            {
                int totalHit = 0;
                IndexManager indexManager =
                    new IndexManager(Server.MapPath(IndexPath), 
                        Server.MapPath(DictPath));
                viewModel.resultList = indexManager.SearchFromIndexDataByPage(keyWords, page, 20, out totalHit);
                viewModel.TotalHit = totalHit;

                viewModel.AllCount = totalHit;
                viewModel.PageIndex = page;
                viewModel.PageStep = 10;
                viewModel.PageSize = 20;
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
                LogService.Log("搜索失败---"+e.Message,e.ToString());
            }
            return View(viewModel);
        }

        public ActionResult Result(string keyWords, int page = 1)
        {
            const string sUrl = "/Search/{0}/{1}";
            Response.Redirect(string.Format(sUrl,keyWords,page));
            return View();
        }
    }
}
