using System;

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