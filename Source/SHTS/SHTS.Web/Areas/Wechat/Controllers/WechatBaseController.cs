using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Controllers;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class WechatBaseController : BaseController
    {
        public override bool RequireLogin()
        {
            if (UserInfo == null)
            {
                Response.Redirect("/wechat/account/login");
                return true;
            }
            return false;
        }
    }
}
