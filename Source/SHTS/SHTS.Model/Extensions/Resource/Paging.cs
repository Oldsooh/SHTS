using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model.Extensions
{
    public class Paging
    {
        public int PageCount { get; set; }

        public int PageStep { get; set; }

        public int PageIndex { get; set; }

        /// <summary>
        /// <example>spacelist</example>
        /// </summary>
        public string ActionName { get; set; }

        public Dictionary<string, string> SelectedFilters { get; set; }
    }
}
