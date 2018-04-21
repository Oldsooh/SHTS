using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class DemandQuoteModel : BaseModel
    {
        public List<DemandQuote> Quotes { get; set; } = new List<DemandQuote>();
        public Demand Demand { get; set; }

        public List<Resource> PostedResources { get; set; } = new List<Resource>();
        public string ActionName { get; set; }
    }
}