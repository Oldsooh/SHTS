using System.ComponentModel.DataAnnotations;

namespace Witbird.SHTS.Web.Models
{
    public class GuestBookViewModel
    {
        /// <summary>
        /// User Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 称呼
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 留言内容
        /// </summary>
        [Required(ErrorMessage = "您还没有输入内容")]
        public string Content { get; set; }
    }
}