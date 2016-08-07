using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class DemandQuoteModel : BaseModel
    {
        public List<DemandQuote> Quotes { get; set; }
        public string ActionName { get; set; }
    }
}