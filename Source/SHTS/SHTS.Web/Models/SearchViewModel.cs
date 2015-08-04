using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WitBird.Com.SearchEngine;

namespace Witbird.SHTS.Web.Models
{
    public class SearchViewModel : BaseModel
    {
        public List<MetaSource> resultList { set; get; }

        public int TotalHit { set; get; }

        public string KewWords { set; get; }
    }
}