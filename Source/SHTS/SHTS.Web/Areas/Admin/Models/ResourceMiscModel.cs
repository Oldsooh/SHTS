using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.DAL.New;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class ResourceMiscModel : BaseModel
    {
        public List<SpaceType> SpaceTypeList { get; } = new List<SpaceType>();

        public List<ActorType> ActorTypeList { get; } = new List<ActorType>();

        public List<EquipType> EquipTypeList { get; } = new List<EquipType>();

        public List<OtherType> OtherTypeList { get; } = new List<OtherType>();
    }
}