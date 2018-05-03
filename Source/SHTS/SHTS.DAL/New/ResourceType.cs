using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.DAL.New
{
    public class ResourceType
    {
        public string ResourceTypeKey { get; set; }
        public string ResourceTypeName { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool MarkForDelete { get; set; }
    }
}
