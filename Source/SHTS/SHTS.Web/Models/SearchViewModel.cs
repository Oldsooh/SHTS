using System.Collections.Generic;
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