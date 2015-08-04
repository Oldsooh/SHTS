using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Authorize
{
    public class AdminHttpContext
    {
        #region 登陆用户信息

        public static String Name
        {
            get
            {
                var userInfo=System.Web.HttpContext.Current.Session["USERINFO"] as AdminUser;
                return userInfo == null ? string.Empty : userInfo.UserName;
            }
        }

        #endregion
    }
}