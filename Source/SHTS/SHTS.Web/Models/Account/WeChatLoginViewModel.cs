using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Witbird.SHTS.Web.Models.Account
{
    public class WeChatLoginViewModel : LoginViewModel
    {
        public Model.User User {get;set;}
    }
}