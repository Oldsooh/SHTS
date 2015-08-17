using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models.User
{
    public class WeChatUserViewModel : UserViewModel
    {
        public Model.WeChatUser WeChatUserEntity { set; get; }
    }
}