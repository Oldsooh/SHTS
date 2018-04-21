using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class DemandQuoteModel : BaseModel
    {
        public List<DemandQuote> Quotes { get; set; }
        public string ActionName { get; set; }
    }
}