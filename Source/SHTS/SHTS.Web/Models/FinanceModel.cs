using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class FinanceModel
    {
        public Model.User CurrentUser { get; set; }
        public FinanceBalance UserBalance { get; set; }
        public List<FinanceWithdrawRecord> WithdrawRecords { get; set; }
        public List<FinanceRecord> FinanceRecords { get; set; }
    }
}