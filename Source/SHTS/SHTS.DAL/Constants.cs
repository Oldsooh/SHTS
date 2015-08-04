using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.DAL
{
    /// <summary>
    /// 数据库访问常量。
    /// </summary>
    public class Constants
    {
        #region 列名参数

        public const string column_UserId = "@UserId";
        public const string column_cellphone = "@Cellphone";

        public const string StartRowIndex = "@startRowIndex";
        public const string PageSize = "@PageSize";
        public const string QueryType = "@QueryType";
        public const string totalCount = "@totalCount";

        #endregion

        #region 列名

        public const string column_totalCount = "totalCount";

        #endregion

    }
}
