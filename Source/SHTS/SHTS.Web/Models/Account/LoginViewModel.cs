using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Witbird.SHTS.Web.Models.Account
{
    public class LoginViewModel
    {
        public string username { set; get; }
        public string password { set; get; }
        public string code { set; get; }
        public string ErrorMsg { set; get; }
        public string RemberAccount { set; get; }
        public string IsAutoLogin { set; get; }
    }
}