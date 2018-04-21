using System.Collections.Generic;

namespace Witbird.SHTS.Model.Extensions
{
    public class UserFilter
    {
        /// <summary>
        /// 已经选择了的过滤器
        /// </summary>
        public Dictionary<string, string> SelectedFilter { get; set; }

        /// <summary>
        /// 没有选择的过滤器
        /// </summary>
        public Dictionary<string, string> UnselectFilter { get; set; }

        /// <summary>
        /// <example>spacelist</example>
        /// </summary>
        public string ActionName { get; set; }
    }
}
