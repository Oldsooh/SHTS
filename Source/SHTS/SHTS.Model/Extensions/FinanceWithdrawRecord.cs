using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model
{
    public partial class FinanceWithdrawRecord
    {
        public string UserName { get; set; }

        public decimal UserAvailableBalance { get; set; }
    }
}
