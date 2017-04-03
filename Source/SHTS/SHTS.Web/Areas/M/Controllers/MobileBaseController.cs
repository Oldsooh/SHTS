using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Controllers;

namespace Witbird.SHTS.Web.Areas.M.Controllers
{
    public class MobileBaseController : BaseController
    {
        public override bool RequireLogin()
        {
            if (UserInfo == null)
            {
                Response.Redirect("/m/account/login");
                return true;
            }
            return false;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //filterContext.Result = Redirect("/mobile");
        }
    }
}
