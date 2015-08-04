using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    [Permission(EnumRole.Normal, EnumBusinessPermission.None)]
    public class SiteBIController : AdminBaseController
    {
        //
        // GET: /Admin/SiteBI/

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult QuerResult(DateTime fromtime, DateTime toTime,string operat=""
            ,int querytype = -1, int page = 1)
        {
            SiteBIViewModel viewModel=new SiteBIViewModel();
            try
            {
                AccessAnalyticsService service = new AccessAnalyticsService();
                int totalCount;
                viewModel.AccessList = service.QuersAccess(fromtime, toTime, page, 10,
                    querytype, out totalCount);
                viewModel.PageSize = 10;
                viewModel.CurrentPage = page;
                viewModel.TotalCount = totalCount;
                viewModel.formTime = fromtime;
                viewModel.toTime = toTime;
                viewModel.QueryType = querytype;
                viewModel.Operat = operat;
            }
            catch (Exception ex)
            {
                LogService.Log("会员列表", ex.StackTrace);
            }
            return PartialView(viewModel);
        }
    }
}
