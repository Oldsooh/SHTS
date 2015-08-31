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

        private List<Resource> items = new List<Resource>();
        public List<Resource> Items 
        {
            get
            {
                return items;
            }

            set
            {
                items = value;
            }
        }

        private Paging paging = new Paging();
        public Paging Paging 
        {
            get
            {
                return paging;
            }
            set
            {
                paging = value;
            }
        }

        private UserFilter filter = new UserFilter();
        public UserFilter Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
            }
        }
    }
}
