using System;

namespace Witbird.SHTS.Web.Models.User
{
    public class UserSummary
    {
        public string UserName { set; get; }
        public string Password { set; get; }
        public DateTime LastLoginTime { set; get; }
    }
}