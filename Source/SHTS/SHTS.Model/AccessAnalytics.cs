//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Witbird.SHTS.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AccessAnalytics
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public string AccessUrl { get; set; }
        public string ReferrerUrl { get; set; }
        public string Operation { get; set; }
        public string PageTitle { get; set; }
        public string IP { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string Agent { get; set; }
        public string Device { get; set; }
    }
}
