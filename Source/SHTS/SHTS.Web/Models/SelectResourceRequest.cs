using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Witbird.SHTS.BLL;

namespace Witbird.SHTS.Web.Models
{
    public class SelectResourceRequest
    {
        [Required(ErrorMessage = "请选择资源类型")]
        [RegularExpression("[0123]", ErrorMessage = "您选择的资源类型不存在")]
        public string ResType { get; set; }
    }
}