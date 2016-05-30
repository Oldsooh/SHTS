using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model
{
    public partial class Demand
    {
        public string CategoryName { get; set; }

        public Int64 RowNumber { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Gets the demand status dispaly value
        /// </summary>
        public string StatusValueString 
        {
            get
            {
                if (!Status.HasValue || Status.Value == (int)DemandStatus.InProgress)
                {
                    return "进行中";
                }
                else if (Status.Value == (int)DemandStatus.Complete)
                {
                    return "已完成";
                }

                return "正在进行";
            }
        }

        public bool InProgress
        {
            get
            {
                return !Status.HasValue || Status.Value == (int)DemandStatus.InProgress;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Status.HasValue && Status.Value == (int)DemandStatus.Complete;
            }
        }
    }
}
