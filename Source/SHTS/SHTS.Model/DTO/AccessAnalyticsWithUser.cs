using System;

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
