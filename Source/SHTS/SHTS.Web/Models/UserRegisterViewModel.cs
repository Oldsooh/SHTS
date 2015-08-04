using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    /// <summary>
    /// 用户注册模型
    /// </summary>
    public class UserRegisterViewModel
    {
        public Witbird.SHTS.Model.User User { set; get; }

        public Witbird.SHTS.Model.SinglePage RegNotice { set; get; }

        public string ErrorMsg { set; get; }
    }
}