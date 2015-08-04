using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.M.Models
{
    public class TradeModel
    {
        public Trade Trade { get; set; }

        public List<Trade> Trades { get; set; }
    }
}