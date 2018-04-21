using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class AdminUserViewModel
    {
        public List<AdminRole> AllRoles { get; set; }

        public List<AdminUser> AllAdminUsers { get; set; }
    }
}