using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class DemandModel : BaseModel
    {
        public Demand Demand { get; set; }

        public List<Demand> Demands { get; set; }

        public DemandCategory demandCategory { get; set; }

        public List<DemandCategory> DemandCategories { get; set; }
    }
}