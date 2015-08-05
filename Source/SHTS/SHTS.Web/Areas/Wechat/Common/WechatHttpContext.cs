using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Common
{
    public class WechatHttpContext
    {
        public static bool IsLogin
        {
            get
            {
                return System.Web.HttpContext.Current.Session["userinfo"] != null;
            }
        }

        public static User UserInfo
        {
            get
            {
                return (User)System.Web.HttpContext.Current.Session["userinfo"];
            }
        }
    }
}