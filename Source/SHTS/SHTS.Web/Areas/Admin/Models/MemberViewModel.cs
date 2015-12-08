using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    /// <summary>
    /// 会员视图
    /// </summary>
    public class MemberViewModel
    {
        public List<User> UserList { set; get; }

        public User SingleUser { set; get; }

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

        public int PageStep { get; set; }

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

        public List<UserVip> VipInfos { get; set; }

        #region 其它

        public string City { set; get; }

        public int Resurce { set; get; }

        public string Keyword { set; get; }

        #endregion
    }
}