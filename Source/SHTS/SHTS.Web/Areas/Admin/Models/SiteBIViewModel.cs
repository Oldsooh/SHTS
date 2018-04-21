using System;
using System.Collections.Generic;
using Witbird.SHTS.Model.DTO;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class SiteBIViewModel
    {
        public List<AccessAnalyticsWithUser> AccessList { set; get; }

        public AccessAnalyticsWithUser SingleData { set; get; }

        public int CurrentPage { set; get; }

        public int PrePageIndex
        {
            get { return CurrentPage == 1 ? 1 : CurrentPage - 1; }
        }

        public int NextPageIndex
        {
            get { return CurrentPage == TotalPage ? CurrentPage : CurrentPage + 1; }
        }

        public int TotalCount { set; get; }

        public int PageSize { set; get; }

        public int QueryType { set; get; }

        public int TotalPage
        {
            get
            {
                int totalPages = 0;
                totalPages = TotalCount % PageSize == 0 ?
                    TotalCount / PageSize : TotalCount / PageSize + 1;
                return totalPages;
            }
        }

        #region 其它

        public DateTime formTime { set; get; }

        public DateTime toTime { set; get; }

        public String Operat { set; get; }

        #endregion
    }
}