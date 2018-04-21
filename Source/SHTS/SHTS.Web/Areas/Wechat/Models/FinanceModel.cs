using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Wechat.Models
{
    public class FinanceModel
    {
        public User CurrentUser { get; set; }
        public WeChatUser CurrentWechatUser { get; set; }
        public FinanceBalance UserBalance { get; set; }
        public List<FinanceWithdrawRecord> WithdrawRecords { get; set; }
        public List<FinanceRecord> FinanceRecords { get; set; }
    }
}