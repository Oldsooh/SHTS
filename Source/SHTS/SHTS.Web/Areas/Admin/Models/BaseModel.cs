namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class BaseModel
    {
        public string Title { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 每页显示页码数
        /// </summary>
        public int PageStep { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int AllCount { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string StartPrice { get; set; }

        public string EndPrice { get; set; }

        public string OrderBy { get; set; }

        public string SC { get; set; }
    }
}