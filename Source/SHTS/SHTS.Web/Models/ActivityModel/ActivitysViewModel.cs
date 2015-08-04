using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models.ActivityModel
{
    public class ActivitysViewModel
    {
        public const string SPLINT = "|";

        public Activity Activity { get; set; }

        public List<Activity>  ActivityList { set; get; }

        public List<Activity> Slides
        {
            get
            {
                if (ActivityList!=null && ActivityList.Count>0)
                {
                    return ActivityList.Take(5).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<ActivityType> ActivityTypes { get; set; }

        public List<Activity> PreTypeActivityList { set; get; }

        /// <summary>
        /// 省份
        /// </summary>
        public List<City> Provinces { get; set; }

        /// <summary>
        /// 一级城市
        /// </summary>
        public List<City> Cities { get; set; }

        /// <summary>
        /// 二级城市、区域、商圈
        /// </summary>
        public List<City> Areas { get; set; }

        #region 参数

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

        public DateTime StartTime { set; get; }

        public DateTime EndTime { set; get; }

        #endregion

        public string GetTypeName(string typeid)
        {
            const string defaultType = "活动";
            string typeName = defaultType;
            if (ActivityTypes != null && ActivityTypes.Count > 0)
            {
                var activityType = ActivityTypes.Find(t => t.ActivityTypeId.ToString() == typeid);
                if (activityType != null)
                {
                    typeName = activityType.ActivityTypeName;
                }
            }
            return typeName;
        }
    }
}