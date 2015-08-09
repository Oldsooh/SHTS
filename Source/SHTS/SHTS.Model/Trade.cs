//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Witbird.SHTS.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Trade
    {
        public int TradeId { get; set; }
        public int UserId { get; set; }
        public string UserQQ { get; set; }
        public string UserCellPhone { get; set; }
        public string UserEmail { get; set; }
        public string UserBankInfo { get; set; }
        public string UserAddress { get; set; }
        public int SellerId { get; set; }
        public int BuyerId { get; set; }
        public decimal TradeAmount { get; set; }
        public string TradeSubject { get; set; }
        public string TradeBody { get; set; }
        public int Payer { get; set; }
        public decimal PayCommission { get; set; }
        public double PayCommissionPercent { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public System.DateTime LastUpdatedTime { get; set; }
        public int State { get; set; }
        public decimal BuyerPay { get; set; }
        public decimal SellerGet { get; set; }
        public int ViewCount { get; set; }
        public string ResourceUrl { get; set; }
        public bool IsBuyerPaid { get; set; }
        public string TradeOrderId { get; set; }
    }
}