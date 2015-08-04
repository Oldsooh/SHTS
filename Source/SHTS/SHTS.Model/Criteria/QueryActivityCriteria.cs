using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model.Criteria
{
    /// <summary>
    /// 查询活动信息条件集合
    /// </summary>
    [Serializable]
    public class QueryActivityCriteria
    {
        public int UserId { set; get; }

        public int StartRowIndex { set; get; }

        public int PageSize { set; get; }

        public int ActivityType { set; get; }

        public int QueryType { set; get; }

        public string Keyword { set; get; }

        public string cityid { set; get; }

        public DateTime? StartTime { set; get; }

        public DateTime? EndTime { set; get; }

        public DateTime? LastUpdatedTime { set; get; }

        public string Status { set; get; }

        public int ResultTotalCount { set; get; }
    }
}
