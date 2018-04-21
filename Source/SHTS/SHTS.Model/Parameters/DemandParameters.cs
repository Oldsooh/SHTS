using System;

namespace Witbird.SHTS.Model
{
    public class DemandParameters
    {
        public int PageCount { get; set; }

        public int PageIndex { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Area { get; set; }

        public string ResourceType { get; set; }

        public string ResourceTypeId { get; set; }

        //public string StartBudget { get; set; }

        //public string EndBudget { get; set; }

        public string BudgetCondition { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public DateTime InsertTime { get; set; }

        /// <summary>
        /// 主持人，歌手。
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// zhixiashi_beijing_dongchengqu, sichuan_chengdu_
        /// </summary>
        public string Locations { get; set; }
        /// <summary>
        /// 1,2,3,4
        /// </summary>
        public string Categories { get; set; }
    }
}
