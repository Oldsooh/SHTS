using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Witbird.SHTS.Web.Models.Account
{
    /// <summary>
    /// 找回密码模型
    /// </summary>
    public class GetBackPasswordViewModel
    {
        public string CellPhone { set; get; }

        public string EncryptedPassword { set; get; }

        public string CellPhoneVCode { set; get; }

        public string VCode { set; get; }

        public string Message { get; set; }
    }
}