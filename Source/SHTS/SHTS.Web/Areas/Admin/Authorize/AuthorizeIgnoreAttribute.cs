using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Witbird.SHTS.Web.Areas.Admin.Authorize
{
    /// <summary>
    /// Attribute for power Authorize
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AuthorizeIgnoreAttribute : Attribute
    {
        public AuthorizeIgnoreAttribute()
        {
        }
    }
}