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
    
    public partial class FinanceOrder
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public System.DateTime InsertedTimestamp { get; set; }
        public System.DateTime LastUpdateTimestamp { get; set; }
    }
}