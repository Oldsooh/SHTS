using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.DAL.New;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class ResourceMiscModel : BaseModel
    {
        public List<ResourceType> ResourceTypes { get; } = new List<ResourceType>();
        public string ResourceTypeKey { get; set; } = "";
    }
}