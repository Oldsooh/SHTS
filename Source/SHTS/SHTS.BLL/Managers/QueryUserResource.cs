using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.BLL.Managers
{
    /// <summary>
    /// 查询某用户创建的资源
    /// </summary>
    public class QueryUserResource : QueryBase
    {
        public int UserId { get; set; }

        public DateTime LastUpdatedTime { set; get; }
    }
}
