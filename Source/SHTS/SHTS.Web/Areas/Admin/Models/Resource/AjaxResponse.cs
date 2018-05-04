namespace Witbird.SHTS.Web.Areas.Admin.Models.Resource
{
    /// <summary>
    /// Ajax请求的返回结果
    /// </summary>
    public class AjaxResponse<T>
    {
        /// <summary>
        /// 状态码
        /// 默认0表示成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public T Data { get; set; }
    }

    public class AjaxResponse :AjaxResponse<string>
    {

    }

}