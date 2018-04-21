using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class FinanceModel : BaseModel
    {
        public List<FinanceWithdrawRecord> WithdrawRecords { get; set; }
    }
}