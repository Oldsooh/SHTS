using System.ComponentModel.DataAnnotations;

namespace Witbird.SHTS.Web.Models
{
    public class CommentViewModel
    {
        [Required(ErrorMessage = "资源不存在")]
        [RegularExpression(@"\d+", ErrorMessage = "资源格式正确")]
        public string ResourceId { get; set; }

        [Required(ErrorMessage = "请填写评论信息")]
        public string Content { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string Title { get; set; }
    }
}