using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model.DTO
{
    /// <summary>
    /// 复杂类型
    /// </summary>
    public class AccessAnalyticsWithUser : AccessAnalytics
    {
        public String UserName { set; get; }
    }
}
