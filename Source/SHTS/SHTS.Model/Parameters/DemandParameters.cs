using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model
{
    public class DemandParameters
    {
        public int PageCount { get; set; }

        public int PageIndex { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Area { get; set; }

        public string Category { get; set; }

        public string StartBudget { get; set; }

        public string EndBudget { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}
