using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model.Extensions
{
    public class QueryResourceResult
    {
        /// <summary>
        /// 1
        /// 2
        /// 3
        /// 4
        /// </summary>
        public int ResourceType { get; set; }
        public string ResourceTypeName { get;set; }

        public int TotalCount { get; set; }

        public List<Resource> Items { get; set; }

        public Paging Paging { get; set; }

        public UserFilter Filter { get; set; }
    }
}
